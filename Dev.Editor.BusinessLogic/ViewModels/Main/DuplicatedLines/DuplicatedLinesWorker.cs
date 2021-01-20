using Dev.Editor.BusinessLogic.Models.DuplicatedLines;
using Dev.Editor.BusinessLogic.Services.TextComparison;
using Dev.Editor.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.ViewModels.Main.DuplicatedLines
{
    public class DuplicatedLinesWorker : BackgroundWorker
    {
        private readonly DuplicatedLinesFinderConfig config;
        private readonly int stepSizePercent;
        private readonly ITextComparisonService textComparisonService;

        // Private types ------------------------------------------------------

        private class SourceFile
        {
            public SourceFile(string path)
            {
                Path = path;
            }

            public string Path { get; }
        }

        private class LineInfo
        {
            public LineInfo(string line, string originalLine)
            {
                Line = line;
                OriginalLine = originalLine;
                LineReferences = new List<LineReference>();
            }

            public string Line { get; }
            public string OriginalLine { get; }
            public List<LineReference> LineReferences { get; }
        }

        private class LineReference
        {
            public LineReference(SourceFile file, int lineNumber, LineInfo line)
            {
                File = file;
                LineNumber = lineNumber;
                Line = line;
            }

            public SourceFile File { get; }
            public int LineNumber { get; }
            public LineInfo Line { get; }
            public LineReference Previous { get; set; }
            public LineReference Next { get; set; }
        }

        private class FilenameComparer : IEqualityComparer<DuplicatedLinesFileInfo>
        {
            public bool Equals(DuplicatedLinesFileInfo x, DuplicatedLinesFileInfo y)
            {
                return x.Path == y.Path;
            }

            public int GetHashCode(DuplicatedLinesFileInfo obj)
            {
                return obj.Path.GetHashCode();
            }
        }

        // Private methods ----------------------------------------------------

        private Dictionary<int, List<LineInfo>> LoadLines()
        {
            var lines = new Dictionary<int, List<LineInfo>>();
            var files = new List<SourceFile>();

            int totalLines = 0;
            int uniqueLines = 0;

            List<WildcardPattern> excludePatterns = config.ExcludeMasks?.Split(';')
                .Select(m => new WildcardPattern(m, WildcardOptions.IgnoreCase))
                .ToList()
                ?? new List<WildcardPattern>();

            for (int sourceIndex = 0; sourceIndex < config.SourcePaths.Count; sourceIndex++)
            {
                if (CancellationPending)
                    return null;

                var path = config.SourcePaths[sourceIndex];

                ReportProgress(sourceIndex * stepSizePercent / config.SourcePaths.Count, String.Format(Strings.FindDuplicatedLines_FileLoadProgress, path));

                if (excludePatterns.Any(p => p.IsMatch(path)))
                    continue;

                var sourceFile = new SourceFile(path);

                LineReference previousReference = null;

                string[] sourceLines = File.ReadAllLines(path);
                for (int lineIndex = 0; lineIndex < sourceLines.Length; lineIndex++)
                {
                    totalLines++;
                    
                    // Ignore excluded lines
                    if (config.LineExclusionRegex?.IsMatch(sourceLines[lineIndex]) ?? false)
                        continue;

                    // Ignore empty lines
                    if (String.IsNullOrWhiteSpace(sourceLines[lineIndex]))
                        continue;

                    string line;

                    if (config.Trim)
                        line = sourceLines[lineIndex].Trim();
                    else
                        line = sourceLines[lineIndex];

                    LineInfo lineInfo;

                    var hash = sourceLines[lineIndex].GetHashCode();
                    if (!lines.TryGetValue(hash, out List<LineInfo> lineInfos))
                    {
                        lineInfos = new List<LineInfo>();
                        lines[hash] = lineInfos;
                    }

                    lineInfo = lineInfos.FirstOrDefault(l => l.Line == line);
                    if (lineInfo == null)
                    {
                        uniqueLines++;
                        lineInfo = new LineInfo(line, sourceLines[lineIndex]);
                        lineInfos.Add(lineInfo);
                    }

                    var reference = new LineReference(sourceFile, lineIndex + 1, lineInfo);
                    lineInfo.LineReferences.Add(reference);

                    if (previousReference != null)
                    {
                        reference.Previous = previousReference;
                        previousReference.Next = reference;
                    }

                    previousReference = reference;
                }
            }

            return lines;
        }

        private void ExpandChain(List<LineInfo> chain, List<LineReference> references, List<DuplicatedLinesResultEntry> results)
        {
            var referenceGroups = references.Select(r => r.Next)
                .Where(x => x != null)
                .GroupBy(r => r.Line)
                .ToList();

            if (referenceGroups.Count > 1 ||
                (referenceGroups.Count == 1 && referenceGroups.Single().Count() < references.Count) ||
                (referenceGroups.Count == 0))
            {
                int files = references.Count;
                int lines = chain.Count;

                if (files >= config.MinFiles && lines >= config.MinLines)
                {
                    var filenames = references.Select(r => new DuplicatedLinesFileInfo(r.File.Path, r.LineNumber - lines + 1, r.LineNumber))
                        .ToList();
                    var text = new List<string>(chain.Select(li => li.OriginalLine));

                    var result = new DuplicatedLinesResultEntry(filenames, text);
                    results.Add(result);
                }
            }

            foreach (var reference in referenceGroups.Where(gr => gr.Count() >= 2))
            {
                List<LineInfo> newChain = new List<LineInfo>(chain);
                newChain.Add(reference.Key);

                ExpandChain(newChain, reference.ToList(), results);
            }
        }

        private List<DuplicatedLinesResultEntry> FindDuplicatedLines(Dictionary<int, List<LineInfo>> lines)
        {
            var result = new List<DuplicatedLinesResultEntry>();

            int total = lines.Select(kvp => kvp.Value.Count).Sum();
            int current = -1;

            foreach (var kvp in lines)
                foreach (var lineInfo in kvp.Value)
                {
                    if (CancellationPending)
                        return null;

                    current++;

                    ReportProgress(stepSizePercent + stepSizePercent * current / total, String.Format(Strings.FindDuplicatedLines_AnalyzingProgress, current + 1, total));

                    // If line is unique, just continue
                    if (lineInfo.LineReferences.Count < 2)
                        continue;

                    // Check, if this line starts an unique chain.
                    var previousLineReferenceGroups = lineInfo.LineReferences
                        .Select(lr => lr.Previous)
                        .Where(x => x != null)
                        .GroupBy(lr => lr.Line)
                        .ToList();

                    // New chain starts, when this line is a first line of
                    // duplicated lines in separate files. This means, that:
                    // * This is a first line of all file, or
                    // * At least one of previous lines differs from others.
                    //
                    //    zzzzX
                    //    aaaaa <- Beginning of chain
                    //    bbbbb

                    if (previousLineReferenceGroups.Count == 1 && previousLineReferenceGroups.Single().Count() == lineInfo.LineReferences.Count)
                        continue;

                    List<LineInfo> chain = new List<LineInfo> { lineInfo };
                    List<LineReference> references = new List<LineReference>(lineInfo.LineReferences);

                    ExpandChain(chain, references, result);
                }

            return result;
        }

        private void MergeCommonResults(List<DuplicatedLinesResultEntry> result)
        {
            for (int i = 0; i < result.Count - 1; i++)
            {
                if (CancellationPending)
                    return;

                var originalEntry = result[i];
                var newEntry = result[i];
                
                ReportProgress(2 * stepSizePercent + stepSizePercent * i / result.Count, String.Format(Strings.FindDuplicatedLines_MergingCommonResults, i + 1, result.Count));

                int j = i + 1;
                while (j < result.Count)
                {
                    // Check, if i-th and j-th results are similar

                    // If line counts differs too much, results cannot be similar
                    if (Math.Abs(originalEntry.Lines.Count - result[j].Lines.Count) > config.AllowedDifferentLines)
                    {
                        j++;
                        continue;
                    }

                    var comparisonResult = textComparisonService.FindChanges(originalEntry.Lines, result[j].Lines, false, config.Trim);

                    var commonLines = comparisonResult.ChangesA.Concat(comparisonResult.ChangesB).Count(x => !x);
                    var differingLines = comparisonResult.ChangesA.Length + comparisonResult.ChangesB.Length - commonLines;
                    
                    if (commonLines >= 2 * config.MinLines && differingLines < config.AllowedDifferentLines)
                    {
                        // Merge j-th result into common result, remove j-th result
                        var uniqueFiles = newEntry.Filenames.Union(result[j].Filenames).Distinct(new FilenameComparer());
                        var files = new List<DuplicatedLinesFileInfo>();

                        foreach (var file in uniqueFiles)
                        {
                            var left = newEntry.Filenames.FirstOrDefault(f => f.Path == file.Path);
                            var right = result[j].Filenames.FirstOrDefault(f => f.Path == file.Path);

                            var entries = new[] { left, right };

                            var newFile = new DuplicatedLinesFileInfo(file.Path,
                                entries.Where(e => e != null).Min(e => e.StartLine),
                                entries.Where(e => e != null).Max(e => e.EndLine));
                            files.Add(newFile);
                        }

                        newEntry = new DuplicatedLinesResultEntry(files,
                            result[i].Lines.Count > result[j].Lines.Count ? result[i].Lines : result[j].Lines);

                        result.RemoveAt(j);
                    }
                    else
                        j++;
                }

                if (originalEntry != newEntry)
                    result[i] = newEntry;
            }
        }

        // Protected methods --------------------------------------------------

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            var lines = LoadLines();
            if (CancellationPending)
                return;

            var result = FindDuplicatedLines(lines);
            if (CancellationPending)
                return;

            if (config.MergeCommonResults)
            {
                MergeCommonResults(result);
            }

            e.Result = new DuplicatedLinesResult(result, config);
        }

        // Public methods -----------------------------------------------------

        public DuplicatedLinesWorker(DuplicatedLinesFinderConfig config, ITextComparisonService textComparisonService)
        {
            this.config = config;
            this.textComparisonService = textComparisonService;

            this.stepSizePercent = config.MergeCommonResults ? 33 : 50;
            this.WorkerSupportsCancellation = true;
            this.WorkerReportsProgress = true;
        }
    }
}

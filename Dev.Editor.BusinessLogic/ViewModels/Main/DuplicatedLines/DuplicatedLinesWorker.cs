using Dev.Editor.BusinessLogic.Models.DuplicatedLines;
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

                ReportProgress(sourceIndex * 50 / config.SourcePaths.Count, String.Format(Strings.FindDuplicatedLines_FileLoadProgress, path));

                if (excludePatterns.Any(p => p.IsMatch(path)))
                    continue;

                var sourceFile = new SourceFile(path);

                LineReference previousReference = null;

                string[] sourceLines = File.ReadAllLines(path);
                for (int lineIndex = 0; lineIndex < sourceLines.Length; lineIndex++)
                {
                    totalLines++;
                    
                    if (config.LineExclusionRegex?.IsMatch(sourceLines[lineIndex]) ?? false)
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
                    ReportProgress(50 + 50 * current / total, String.Format(Strings.FindDuplicatedLines_AnalyzingProgress, current + 1, total));

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

        // Protected methods --------------------------------------------------

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            var lines = LoadLines();
            if (CancellationPending)
                return;

            var result = FindDuplicatedLines(lines);
            if (CancellationPending)
                return;

            e.Result = new DuplicatedLinesResult(result, config);
        }

        // Public methods -----------------------------------------------------

        public DuplicatedLinesWorker(DuplicatedLinesFinderConfig config)
        {
            this.config = config;
            this.WorkerSupportsCancellation = true;
            this.WorkerReportsProgress = true;
        }
    }
}

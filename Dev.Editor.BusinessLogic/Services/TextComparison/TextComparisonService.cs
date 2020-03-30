using Dev.Editor.BusinessLogic.Models.TextComparison;
using Dev.Editor.BusinessLogic.Types.TextComparison;
using ICSharpCode.AvalonEdit.Document;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Services.TextComparison
{
    class TextComparisonService : ITextComparisonService
    {
        // Private types ------------------------------------------------------

        private enum LastEdit
        {
            InsertDown,
            DeleteRight,
            InsertUp,
            DeleteLeft
        }

        private class MiddleSnakeResult
        {
            public MiddleSnakeResult(int dPathLength, int startX, int startY, int endX, int endY, LastEdit lastEdit)
            {
                DPathLength = dPathLength;
                StartX = startX;
                StartY = startY;
                EndX = endX;
                EndY = endY;
                LastEdit = lastEdit;
            }

            public override string ToString()
            {
                return $"MiddleSnakeResult: Start ({StartX}, {StartY}), End ({EndX}, {EndY}), DPath length {DPathLength}, Last edit {LastEdit}";
            }

            public int DPathLength { get; }
            public int StartX { get; }
            public int StartY { get; }
            public int EndX { get; }
            public int EndY { get; }
            public LastEdit LastEdit { get; }
        }

        private class TextComparisonContext
        {
            public TextComparisonContext(int[] aData, int[] bData)
            {
                AData = aData;
                BData = bData;
                AChanges = new bool[aData.Length];
                BChanges = new bool[bData.Length];
            }

            public int[] AData { get; }
            public int[] BData { get; }
            public bool[] AChanges { get; }
            public bool[] BChanges { get; }

            public TextComparisonRange FullRange => new TextComparisonRange(0, AData.Length, 0, BData.Length);
        }

        private class TextComparisonRange
        {
            public TextComparisonRange(int startA, int endA, int startB, int endB)
            {
                StartA = startA;
                EndA = endA;
                StartB = startB;
                EndB = endB;
            }

            public int StartA { get; }
            public int EndA { get; }
            public int StartB { get; }
            public int EndB { get; }
        }

        private class DiffBlock
        {
            public DiffBlock(int deleteStartA, int deleteCountA, int insertStartB, int insertCountB)
            {
                DeleteStartA = deleteStartA;
                DeleteCountA = deleteCountA;
                InsertStartB = insertStartB;
                InsertCountB = insertCountB;
            }

            public int DeleteStartA { get; }
            public int DeleteCountA { get; }
            public int InsertStartB { get; }
            public int InsertCountB { get; }
        }

        // Private methods ----------------------------------------------------

        private int[] DocumentToHashLines(TextDocument doc, Hashtable hashedLines, bool ignoreCase, bool ignoreWhitespace)
        {
            var result = new int[doc.LineCount];

            int nextIndex = hashedLines.Count;

            for (int i = 0; i < doc.LineCount; i++)
            {
                string line = doc.GetText(doc.Lines[i].Offset, doc.Lines[i].Length);
                if (ignoreCase)
                    line = line.ToLower();
                if (ignoreWhitespace)
                    line = Regex.Replace(line, "\\s*", " ").Trim();

                if (hashedLines.ContainsKey(line))
                {
                    result[i] = (int)hashedLines[line];
                }
                else
                {
                    var hash = nextIndex++;
                    hashedLines[line] = hash;
                    result[i] = hash;
                }
            }

            return result;
        }

        private List<DiffBlock> EvalDiffBlocks(TextComparisonContext context)
        {
            int posA = 0;
            int posB = 0;

            List<DiffBlock> diffBlocks = new List<DiffBlock>();

            do
            {
                while (posA < context.AChanges.Length && posB < context.BChanges.Length && !context.AChanges[posA] && !context.BChanges[posB])
                {
                    posA++;
                    posB++;
                }

                int beginA = posA;
                int beginB = posB;

                while (posA < context.AChanges.Length && context.AChanges[posA])
                    posA++;

                while (posB < context.BChanges.Length && context.BChanges[posB])
                    posB++;

                int deletedCount = posA - beginA;
                int insertedCount = posB - beginB;
                if (deletedCount > 0 || insertedCount > 0)
                {
                    diffBlocks.Add(new DiffBlock(beginA, deletedCount, beginB, insertedCount));
                }
            }
            while (posA < context.AChanges.Length && posB < context.BChanges.Length);

            return diffBlocks;
        }

        private MiddleSnakeResult FindMiddleSnake(TextComparisonContext context, TextComparisonRange range)
        {
            int N = range.EndA - range.StartA;
            int M = range.EndB - range.StartB;
            int delta = N - M;
            int half = (M + N) / 2 + (M + N) % 2;
            bool deltaIsOdd = delta % 2 == 1;

            var forwardVector = new FreeIndexArray<int>(-half, half);
            var reverseVector = new FreeIndexArray<int>(-half, half);

            forwardVector[1] = 0;
            reverseVector[1] = N + 1;

            LastEdit lastEdit;

            // Iterate through D-path lengths
            for (int D = 0; D <= half; D++)
            {
                // Iterate through diagonals for forward search
                for (int k = -D; k <= D; k += 2)
                {
                    // Find the end of the furthest reaching forward D-path in diagonal k
                    int x;
                    if (k == -D || (k != D && forwardVector[k - 1] < forwardVector[k + 1]))
                    {
                        x = forwardVector[k + 1]; // Going down
                        lastEdit = LastEdit.InsertDown;
                    }
                    else
                    {
                        x = forwardVector[k - 1] + 1; // Going right
                        lastEdit = LastEdit.DeleteRight;
                    }

                    int y = x - k;

                    int startX = x;
                    int startY = y;

                    // Now following the snake as far as possible
                    while (x < N && y < M && context.AData[x + range.StartA] == context.BData[y + range.StartB])
                    {
                        x++;
                        y++;
                    }

                    forwardVector[k] = x;

                    if (deltaIsOdd && k >= delta - (D - 1) && k <= delta + (D - 1))
                    {
                        if (forwardVector[k] >= reverseVector[k - delta])
                        {
                            return new MiddleSnakeResult(2 * D - 1,
                                startX + range.StartA,
                                startY + range.StartB,
                                x + range.StartA,
                                y + range.StartB,
                                lastEdit);
                        }
                    }
                }

                // Iterate through diagonals for reverse search
                for (int k = -D; k <= D; k += 2)
                {
                    // Find the end of the furthest reaching reverse D-path in diagonal k + delta
                    int x;
                    if (k == -D || (k != D && reverseVector[k + 1] <= reverseVector[k - 1]))
                    {
                        x = reverseVector[k + 1] - 1; // Going left
                        lastEdit = LastEdit.DeleteLeft;
                    }
                    else
                    {
                        x = reverseVector[k - 1]; // Going up
                        lastEdit = LastEdit.InsertUp;
                    }

                    int y = x - (k + delta);

                    int endX = x;
                    int endY = y;

                    // Now following the reverse snake as far as possible
                    while (x > 0 && y > 0 && context.AData[x + range.StartA - 1] == context.BData[y + range.StartB - 1])
                    {
                        x--;
                        y--;
                    }

                    reverseVector[k] = x;

                    if (!deltaIsOdd && k + delta >= -D && k + delta <= D)
                    {
                        if (forwardVector[k + delta] >= reverseVector[k])
                        {
                            return new MiddleSnakeResult(2 * D,
                                x + range.StartA,
                                y + range.StartB,
                                endX + range.StartA,
                                endY + range.StartB,
                                lastEdit);
                        }
                    }
                }
            }

            throw new Exception("Broken algorithm - should never reach this place.");
        }

        private TextComparisonContext GenerateComparisonContext(TextDocument documentA, TextDocument documentB, bool ignoreCase, bool ignoreWhitespace)
        {
            Hashtable hashedLines = new Hashtable();

            int[] dataA = DocumentToHashLines(documentA, hashedLines, ignoreCase, ignoreWhitespace);
            int[] dataB = DocumentToHashLines(documentB, hashedLines, ignoreCase, ignoreWhitespace);

            var context = new TextComparisonContext(dataA, dataB);
            return context;
        }

        private string GetLine(TextDocument document, int lineIndex)
        {
            var line = document.GetLineByNumber(lineIndex);
            return document.GetText(line.Offset, line.Length);
        }

        private void InternalEvalDiff(TextComparisonContext context, TextComparisonRange range)            
        {
            int startA = range.StartA;
            int startB = range.StartB;
            int endA = range.EndA;
            int endB = range.EndB;

            // Skip first equal items
            while (startA < endA && startB < endB && context.AData[startA] == context.BData[startB])
            {
                startA++;
                startB++;
            }

            // Skip last equal items
            while (startA < endA && startB < endB && context.AData[endA - 1] == context.BData[endB - 1])
            {
                endA--;
                endB--;
            }

            // If there is anything to work on...
            if (startA < endA && startB < endB)
            {
                MiddleSnakeResult middleSnake = FindMiddleSnake(context, new TextComparisonRange(startA, endA, startB, endB));

                if (middleSnake.DPathLength > 1)
                {
                    int startX = middleSnake.StartX;
                    int startY = middleSnake.StartY;
                    int endX = middleSnake.EndX;
                    int endY = middleSnake.EndY;

                    if (middleSnake.LastEdit == LastEdit.DeleteRight && middleSnake.StartX - 1 > startA)
                    {
                        context.AChanges[middleSnake.StartX - 1] = true;
                        startX--;
                    }
                    else if (middleSnake.LastEdit == LastEdit.InsertDown && middleSnake.StartY - 1 > startB)
                    {
                        context.BChanges[middleSnake.StartY - 1] = true;
                        startY--;
                    }
                    else if (middleSnake.LastEdit == LastEdit.DeleteLeft && middleSnake.EndX < endA)
                    {
                        context.AChanges[middleSnake.EndX] = true;
                        endX++;
                    }
                    else if (middleSnake.LastEdit == LastEdit.InsertUp && middleSnake.EndY < endB)
                    {
                        context.BChanges[middleSnake.EndY] = true;
                        endY++;
                    }

                    InternalEvalDiff(context, new TextComparisonRange(startA, startX, startB, startY));
                    InternalEvalDiff(context, new TextComparisonRange(endX, endA, endY, endB));
                }
            }
            else if (startA < endA)
            {
                for (int i = startA; i < endA; i++)
                {
                    context.AChanges[i] = true;
                }
            }
            else if (startB < endB)
            {
                for (int i = startB; i < endB; i++)
                {
                    context.BChanges[i] = true;
                }
            }
        }

        // Public methods -----------------------------------------------------

        public ChangesResult FindChanges(TextDocument documentA, TextDocument documentB, bool ignoreCase = false, bool ignoreWhitespace = false)
        {
            TextComparisonContext context = GenerateComparisonContext(documentA, documentB, ignoreCase, ignoreWhitespace);
            InternalEvalDiff(context, context.FullRange);

            return new ChangesResult(context.AChanges, context.BChanges);
        }

        public ContinuousLineDiffResult GenerateContinuousLineDiff(TextDocument documentA, TextDocument documentB, bool ignoreCase = false, bool ignoreWhitespace = false)
        {
            TextComparisonContext context = GenerateComparisonContext(documentA, documentB, ignoreCase, ignoreWhitespace);
            InternalEvalDiff(context, context.FullRange);

            var blocks = EvalDiffBlocks(context);

            List<LineDiffInfo> result = new List<LineDiffInfo>();

            int posB = 0;

            foreach (var block in blocks)
            {
                while (posB < block.InsertStartB)
                {
                    
                    result.Add(new LineDiffInfo(ChangeType.Unchanged, GetLine(documentB, posB)));
                    posB++;
                }

                for (int i = 0; i < block.DeleteCountA; i++)
                {
                    result.Add(new LineDiffInfo(ChangeType.Deleted, GetLine(documentA, i + block.DeleteStartA)));
                }

                for (int i = 0; i < block.InsertCountB; i++)
                {
                    result.Add(new LineDiffInfo(ChangeType.Inserted, GetLine(documentB, posB)));
                    posB++;
                }
            }

            while (posB < context.BData.Length)
            {
                result.Add(new LineDiffInfo(ChangeType.Unchanged, GetLine(documentB, posB)));
                posB++;
            }

            return new ContinuousLineDiffResult(result); ;
        }
    }
}

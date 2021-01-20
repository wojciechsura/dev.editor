using ICSharpCode.AvalonEdit.Document;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Services.TextComparison
{
    public class TextDocumentAsList : IReadOnlyList<string>
    {
        private class DocumentIterator : IEnumerator<string>
        {
            private int currentLine;
            private TextDocument doc;

            public DocumentIterator(TextDocument document)
            {
                this.doc = document;
                currentLine = -1;
            }

            public void Dispose()
            {
                currentLine = -1;
            }

            public bool MoveNext()
            {
                return (++currentLine < doc.LineCount);
            }

            public void Reset()
            {
                currentLine = -1;
            }

            public string Current => doc.GetText(doc.Lines[currentLine].Offset, doc.Lines[currentLine].Length);

            object IEnumerator.Current => Current;
        }

        private readonly TextDocument doc;

        public TextDocumentAsList(TextDocument textDocument)
        {
            this.doc = textDocument;
        }

        public int Count => doc.LineCount;

        public string this[int line] => doc.GetText(doc.Lines[line].Offset, doc.Lines[line].Length);

        public IEnumerator<string> GetEnumerator()
        {
            return new DocumentIterator(doc);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new DocumentIterator(doc);
        }
    }
}

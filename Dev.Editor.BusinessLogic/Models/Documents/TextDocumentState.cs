using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.Documents
{
    public class TextDocumentState
    {
        public TextDocumentState(TextEditorState editorState, TextEditorState editorState2)
        {
            EditorState = editorState;
            EditorState2 = editorState2;
        }

        public TextEditorState EditorState { get; }
        public TextEditorState EditorState2 { get; }
    }
}

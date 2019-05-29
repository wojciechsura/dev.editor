using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dev.Editor.BusinessLogic.ViewModels.Interfaces;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;

namespace Dev.Editor.BusinessLogic.ViewModels
{
    public class DocumentViewModel : BaseViewModel
    {
        private readonly TextDocument document;
        private bool changed;

        private void HandleFileNameChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(() => FileName);
        }

        public DocumentViewModel()
        {
            document = new TextDocument();
            document.FileNameChanged += HandleFileNameChanged;                        
        }

        public TextDocument Document => document;

        public string FileName => Path.GetFileName(document.FileName);
            
        public bool Changed
        {
            get => changed;
            set  
            {
                Set(ref changed, () => Changed, value);
            }
        }

        public ITextEditorAccess TextEditorAccess { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;

namespace Dev.Editor.BusinessLogic.ViewModels
{
    public class DocumentViewModel : BaseViewModel
    {
        // Private fields -----------------------------------------------------

        private readonly TextDocument document;
        private bool changed;

        // Private methods ----------------------------------------------------

        private void HandleFileNameChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(() => FileName);
        }

        // Public methods -----------------------------------------------------

        public DocumentViewModel()
        {
            document = new TextDocument();
            document.FileNameChanged += HandleFileNameChanged;                        
        }

        public DocumentViewModel(Stream stream)
            : this()
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                document.Text = reader.ReadToEnd();
            }
        }

        // Public properties --------------------------------------------------

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
    }
}

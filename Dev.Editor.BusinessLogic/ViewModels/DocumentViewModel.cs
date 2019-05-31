using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dev.Editor.BusinessLogic.Models.Documents;
using Dev.Editor.Common.Conditions;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;

namespace Dev.Editor.BusinessLogic.ViewModels
{
    public class DocumentViewModel : BaseViewModel
    {
        // Private fields -----------------------------------------------------

        private readonly TextDocument document;
        private bool changed;
        private bool filenameVirtual;
        private DocumentState storedState;

        // Private methods ----------------------------------------------------

        private void HandleFileNameChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(() => FileName);
        }

        private void LoadFile(Stream stream, string filename)
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                document.Text = reader.ReadToEnd();
                document.FileName = filename;
            }
        }

        // Public methods -----------------------------------------------------

        public DocumentViewModel()
        {
            document = new TextDocument();
            document.FileNameChanged += HandleFileNameChanged;

            storedState = null;
            changed = false;
            filenameVirtual = true;
        }

        public static DocumentViewModel CreateFromFile(Stream stream, string filename)
        {
            var result = new DocumentViewModel();
            result.LoadFile(stream, filename);

            return result;
        }

        public DocumentState LoadState()
        {
            return storedState;
        }

        public void SaveState(DocumentState state)
        {
            storedState = state;
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

        public bool FilenameVirtual
        {
            get => filenameVirtual;
            set
            {
                Set(ref filenameVirtual, () => FilenameVirtual, value);
            } 
        }
    }
}

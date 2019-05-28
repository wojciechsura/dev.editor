using Dev.Editor.BusinessLogic.Services.Documents;
using Dev.Editor.Common.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dev.Editor.BusinessLogic.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        // Private fields -----------------------------------------------------

        private readonly IDocumentManager documentManager;

        // Private methods ----------------------------------------------------

        private void DoSaveAs()
        {
            throw new NotImplementedException();
        }

        private void DoSave()
        {
            throw new NotImplementedException();
        }

        private void DoOpen()
        {
            throw new NotImplementedException();
        }

        private void DoNew()
        {
            throw new NotImplementedException();
        }

        // Protected methods --------------------------------------------------

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        // Public methods -----------------------------------------------------

        public MainWindowViewModel(IDocumentManager documentManager)
        {
            this.documentManager = documentManager;

            NewCommand = new AppCommand(obj => DoNew());
            OpenCommand = new AppCommand(obj => DoOpen());
            SaveCommand = new AppCommand(obj => DoSave());
            SaveAsCommand = new AppCommand(obj => DoSaveAs());

            // TODO (if not opened with parameters)
            documentManager.AddNewDocument();
        }

        // Public properties --------------------------------------------------

        public ReadOnlyObservableCollection<DocumentViewModel> Documents => documentManager.Documents;
        public DocumentViewModel ActiveDocument { get; set; }

        public ICommand NewCommand { get; }
        public ICommand OpenCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand SaveAsCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

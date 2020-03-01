using Dev.Editor.BusinessLogic.Types.Main;
using Dev.Editor.BusinessLogic.ViewModels.Document;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.ViewModels.Main.Documents
{
    public class DocumentsManager : INotifyPropertyChanged
    {
        // Private fields -----------------------------------------------------

        private readonly ObservableCollectionEx<BaseDocumentViewModel> primaryDocuments;
        private readonly ObservableCollectionEx<BaseDocumentViewModel> secondaryDocuments;

        private BaseDocumentViewModel selectedPrimaryDocument;
        private BaseDocumentViewModel selectedSecondaryDocument;
        private DocumentTabKind activeDocumentTab;
        private BaseDocumentViewModel activeDocument;
        private bool showSecondaryDocumentsTab;

        // Private methods ----------------------------------------------------

        private void SetSelectedPrimaryDocument(BaseDocumentViewModel value)
        {
            if (value == selectedPrimaryDocument)
                return;

            selectedPrimaryDocument = value;
            OnPropertyChanged(nameof(SelectedPrimaryDocument));
            
            if (activeDocumentTab == DocumentTabKind.Primary)
            {
                activeDocument = selectedPrimaryDocument;
                OnPropertyChanged(nameof(ActiveDocument));
            }
        }

        private void SetSelectedSecondaryDocument(BaseDocumentViewModel value)
        {
            if (value == selectedSecondaryDocument)
                return;

            selectedSecondaryDocument = value;
            OnPropertyChanged(nameof(SelectedSecondaryDocument));

            if (activeDocumentTab == DocumentTabKind.Secondary)
            {
                activeDocument = selectedSecondaryDocument;
                OnPropertyChanged(nameof(ActiveDocument));
            }
        }

        private void SetActiveDocument(BaseDocumentViewModel value)
        {
            if (value == activeDocument)
                return;

            // Setting active document switches active document tab if needed

            if (primaryDocuments.Contains(value))
            {
                if (activeDocumentTab != DocumentTabKind.Primary)
                {
                    activeDocumentTab = DocumentTabKind.Primary;
                    OnPropertyChanged(nameof(ActiveDocumentTab));
                }

                if (selectedPrimaryDocument != value)
                {
                    selectedPrimaryDocument = value;
                    OnPropertyChanged(nameof(SelectedPrimaryDocument));
                }

                activeDocument = value;
                OnPropertyChanged(nameof(ActiveDocument));
            }
            else if (secondaryDocuments.Contains(value))
            {
                if (activeDocumentTab != DocumentTabKind.Secondary)
                {
                    activeDocumentTab = DocumentTabKind.Secondary;
                    OnPropertyChanged(nameof(ActiveDocumentTab));
                }

                if (selectedSecondaryDocument != value)
                {
                    selectedSecondaryDocument = value;
                    OnPropertyChanged(nameof(SelectedSecondaryDocument));
                }

                activeDocument = value;
                OnPropertyChanged(nameof(ActiveDocument));
            }
            else
                throw new ArgumentException("Document not found!", nameof(value));
        }

        private void SetActiveDocumentTab(DocumentTabKind value)
        {
            if (value == activeDocumentTab)
                return;

            activeDocumentTab = value;
            OnPropertyChanged(nameof(ActiveDocumentTab));

            switch (activeDocumentTab)
            {
                case DocumentTabKind.Primary:
                    {
                        activeDocument = selectedPrimaryDocument;
                        break;
                    }
                case DocumentTabKind.Secondary:
                    {
                        activeDocument = selectedSecondaryDocument;
                        break;
                    }
                default:
                    throw new InvalidEnumArgumentException("Unsupported active document tab!");
            }
            OnPropertyChanged(nameof(ActiveDocument));
        }

        private void SetShowSecondaryDocumentsTab(bool value)
        {
            throw new NotImplementedException();
        }

        // Protected methods --------------------------------------------------

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Public properties --------------------------------------------------

        public BaseDocumentViewModel SelectedPrimaryDocument
        {
            get => selectedPrimaryDocument;
            set => SetSelectedPrimaryDocument(value);
        }

        public BaseDocumentViewModel SelectedSecondaryDocument
        {
            get => selectedSecondaryDocument;
            set => SetSelectedSecondaryDocument(value);
        }

        public BaseDocumentViewModel ActiveDocument
        {
            get => activeDocument;
            set => SetActiveDocument(value);
        }

        public DocumentTabKind ActiveDocumentTab
        {
            get => activeDocumentTab;
            set => SetActiveDocumentTab(value);
        }

        public bool ShowSecondaryDocumentsTab
        {
            get => showSecondaryDocumentsTab;
            set => SetShowSecondaryDocumentsTab(value);
        }

        public IReadOnlyObservableCollection<BaseDocumentViewModel> PrimaryDocuments => primaryDocuments;
        public IReadOnlyObservableCollection<BaseDocumentViewModel> SecondaryDocuments => secondaryDocuments;
        public IEnumerable<BaseDocumentViewModel> AllDocuments => primaryDocuments.Union(secondaryDocuments);

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

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

        private readonly TabDocumentCollection<BaseDocumentViewModel> primaryDocuments;
        private readonly TabDocumentCollection<BaseDocumentViewModel> secondaryDocuments;

        private BaseDocumentViewModel selectedPrimaryDocument;
        private BaseDocumentViewModel selectedSecondaryDocument;
        private DocumentTabKind activeDocumentTab;
        private BaseDocumentViewModel activeDocument;
        private bool showSecondaryDocumentTab;

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
            showSecondaryDocumentTab = value;
            OnPropertyChanged(nameof(ShowSecondaryDocumentTab));

            if (!showSecondaryDocumentTab)
            {
                // Moving documents back to main tab
                while (secondaryDocuments.Count > 0)
                {
                    var doc = secondaryDocuments[secondaryDocuments.Count - 1];
                    secondaryDocuments.RemoveAt(secondaryDocuments.Count - 1);
                    primaryDocuments.Add(doc);
                }

                if (activeDocument != selectedPrimaryDocument)
                {
                    activeDocument = selectedPrimaryDocument;
                    OnPropertyChanged(nameof(ActiveDocument));
                }
            }
        }

        private void InternalRemoveDocumentFrom(BaseDocumentViewModel document,
            TabDocumentCollection<BaseDocumentViewModel> collection,
            ref BaseDocumentViewModel selectedCollectionItem,
            string selectedPropertyName)
        {
            int index = collection.IndexOf(document);
            collection.Remove(document);

            if (selectedCollectionItem == document)
            {
                if (index >= collection.Count)
                    index = collection.Count - 1;

                if (index > 0 && index < collection.Count)
                    selectedCollectionItem = collection[index];
                else
                    selectedCollectionItem = null;

                OnPropertyChanged(selectedPropertyName);

                if (activeDocumentTab == collection.DocumentTabKind && activeDocument != selectedCollectionItem)
                {
                    activeDocument = selectedCollectionItem;
                    OnPropertyChanged(nameof(ActiveDocument));
                }
            }
        }

        private void InternalAddDocumentTo(BaseDocumentViewModel document,
            TabDocumentCollection<BaseDocumentViewModel> collection,
            ref BaseDocumentViewModel selectedCollectionItem,
            string selectedPropertyName)
        {
            InternalInsertDocumentTo(collection.Count,
                document,
                collection,
                ref selectedCollectionItem,
                selectedPropertyName);
        }

        private void InternalInsertDocumentTo(int index,
            BaseDocumentViewModel document,
            TabDocumentCollection<BaseDocumentViewModel> collection,
            ref BaseDocumentViewModel selectedCollectionItem,
            string selectedPropertyName)
        {
            collection.Insert(index, document);

            if (collection.Count == 1)
            {
                selectedCollectionItem = document;
                OnPropertyChanged(selectedPropertyName);
            }
        }

        // Protected methods --------------------------------------------------

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Public methods -----------------------------------------------------

        public DocumentsManager()
        {
            primaryDocuments = new TabDocumentCollection<BaseDocumentViewModel>(DocumentTabKind.Primary);
            secondaryDocuments = new TabDocumentCollection<BaseDocumentViewModel>(DocumentTabKind.Secondary);
            activeDocument = null;
            selectedPrimaryDocument = null;
            selectedSecondaryDocument = null;
            activeDocumentTab = DocumentTabKind.Primary;
            showSecondaryDocumentTab = false;
        }

        public void RemoveDocument(BaseDocumentViewModel document)
        {
            if (primaryDocuments.Contains(document))
                InternalRemoveDocumentFrom(document, 
                    primaryDocuments,
                    ref selectedPrimaryDocument,
                    nameof(SelectedPrimaryDocument));
            else if (secondaryDocuments.Contains(document))
                InternalRemoveDocumentFrom(document,
                    secondaryDocuments,
                    ref selectedSecondaryDocument,
                    nameof(SelectedSecondaryDocument));
            else
                throw new ArgumentException(nameof(document));
        }

        public void RemoveDocument(DocumentTabKind documentTabKind, BaseDocumentViewModel document)
        {
            switch (documentTabKind)
            {
                case DocumentTabKind.Primary:
                    InternalRemoveDocumentFrom(document,
                        primaryDocuments,
                        ref selectedPrimaryDocument,
                        nameof(SelectedPrimaryDocument));
                    break;
                case DocumentTabKind.Secondary:
                    InternalRemoveDocumentFrom(document,
                        secondaryDocuments,
                        ref selectedSecondaryDocument,
                        nameof(SelectedSecondaryDocument));
                    break;
                default:
                    throw new InvalidEnumArgumentException("Unsupported document tab kind!");
            }
        }

        public void AddDocument(BaseDocumentViewModel document, DocumentTabKind documentTabKind)
        {
            switch (documentTabKind)
            {
                case DocumentTabKind.Primary:
                    InternalAddDocumentTo(document, primaryDocuments, ref selectedPrimaryDocument, nameof(SelectedPrimaryDocument));
                    break;
                case DocumentTabKind.Secondary:
                    InternalAddDocumentTo(document, secondaryDocuments, ref selectedSecondaryDocument, nameof(SelectedSecondaryDocument));

                    if (!showSecondaryDocumentTab)
                    {
                        showSecondaryDocumentTab = true;
                        OnPropertyChanged(nameof(showSecondaryDocumentTab));
                    }
                    break;
                default:
                    throw new InvalidEnumArgumentException("Unsupported document tab kind!");
            }
        }

        public void InsertDocument(int index, BaseDocumentViewModel document, DocumentTabKind documentTabKind)
        {
            switch (documentTabKind)
            {
                case DocumentTabKind.Primary:
                    InternalInsertDocumentTo(index, document, primaryDocuments, ref selectedPrimaryDocument, nameof(SelectedPrimaryDocument));
                    break;
                case DocumentTabKind.Secondary:
                    InternalInsertDocumentTo(index, document, secondaryDocuments, ref selectedSecondaryDocument, nameof(SelectedSecondaryDocument));

                    if (!showSecondaryDocumentTab)
                    {
                        showSecondaryDocumentTab = true;
                        OnPropertyChanged(nameof(showSecondaryDocumentTab));
                    }
                    break;
                default:
                    throw new InvalidEnumArgumentException("Unsupported document tab kind!");
            }
        }

        public void ReorderDocument(BaseDocumentViewModel fromDoc, BaseDocumentViewModel toDoc)
        {
            void MoveInside(TabDocumentCollection<BaseDocumentViewModel> documents, 
                BaseDocumentViewModel fromDocument, 
                BaseDocumentViewModel toDocument)
            {
                var fromIndex = documents.IndexOf(fromDocument);
                var toIndex = documents.IndexOf(toDocument);

                documents.Move(fromIndex, toIndex);
            }

            void MoveFromTo(TabDocumentCollection<BaseDocumentViewModel> fromDocuments, BaseDocumentViewModel fromDocument,
                TabDocumentCollection<BaseDocumentViewModel> toDocuments, BaseDocumentViewModel toDocument)
            {
                int fromIndex = fromDocuments.IndexOf(fromDocument);
                int toIndex = toDocuments.IndexOf(toDocument);

                var doc = fromDocuments[fromIndex];
                fromDocuments.RemoveAt(fromIndex);
                toDocuments.Insert(toIndex, doc);
            }

            if (fromDoc == toDoc)
                return;

            if (primaryDocuments.Contains(fromDoc))
            {
                int fromIndex = primaryDocuments.IndexOf(fromDoc);

                if (primaryDocuments.Contains(toDoc))
                {
                    MoveInside(primaryDocuments, fromDoc, toDoc);
                }
                else if (secondaryDocuments.Contains(toDoc))
                {
                    MoveFromTo(primaryDocuments, fromDoc, secondaryDocuments, toDoc);
                }
                else
                    throw new InvalidOperationException("Target document does not exist in internal collections!");
            }
            else if (secondaryDocuments.Contains(fromDoc))
            {
                int fromIndex = secondaryDocuments.IndexOf(fromDoc);

                if (primaryDocuments.Contains(toDoc))
                {
                    MoveFromTo(secondaryDocuments, fromDoc, primaryDocuments, toDoc);
                }
                else if (secondaryDocuments.Contains(toDoc))
                {
                    MoveInside(secondaryDocuments, fromDoc, toDoc);
                }
                else
                    throw new InvalidOperationException("Target document does not exist in internal collections!");
            }
            else
                throw new InvalidOperationException("Target document does not exist in internal collections!");

            SetActiveDocument(fromDoc);
        }

        public void MoveDocumentTo(BaseDocumentViewModel documentViewModel, DocumentTabKind targetDocumentTabKind)
        {
            TabDocumentCollection<BaseDocumentViewModel> sourceDocuments;
            TabDocumentCollection<BaseDocumentViewModel> destinationDocuments;

            switch (targetDocumentTabKind)
            {
                case DocumentTabKind.Primary:
                    destinationDocuments = secondaryDocuments;
                    break;
                case DocumentTabKind.Secondary:
                    destinationDocuments = primaryDocuments;
                    break;
                default:
                    throw new InvalidEnumArgumentException("Unsupported document tab kind!");
            }

            if (primaryDocuments.Contains(documentViewModel))
                sourceDocuments = primaryDocuments;
            else if (secondaryDocuments.Contains(documentViewModel))
                sourceDocuments = secondaryDocuments;
            else
                throw new ArgumentException(nameof(documentViewModel));

            // The only case when nothing needs to be done
            if (sourceDocuments == destinationDocuments && sourceDocuments.IndexOf(documentViewModel) == sourceDocuments.Count - 1)
                return;

            sourceDocuments.Remove(documentViewModel);
            destinationDocuments.Add(documentViewModel);

            SetActiveDocument(documentViewModel);
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

        public bool ShowSecondaryDocumentTab
        {
            get => showSecondaryDocumentTab;
            set => SetShowSecondaryDocumentsTab(value);
        }

        public ITabDocumentCollection<BaseDocumentViewModel> PrimaryDocuments => primaryDocuments;

        public ITabDocumentCollection<BaseDocumentViewModel> SecondaryDocuments => secondaryDocuments;

        public IEnumerable<BaseDocumentViewModel> AllDocuments => primaryDocuments.Union(secondaryDocuments);

        public ITabDocumentCollection<BaseDocumentViewModel> this[DocumentTabKind documentTabKind]
        {
            get
            {
                switch (documentTabKind)
                {
                    case DocumentTabKind.Primary:
                        return primaryDocuments;
                    case DocumentTabKind.Secondary:
                        return secondaryDocuments;
                    default:
                        throw new InvalidEnumArgumentException("Unsupported document tab kind!");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

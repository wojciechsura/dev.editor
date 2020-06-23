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

        private void DoSetSelectedPrimaryDocument(BaseDocumentViewModel value)
        {
            if (selectedPrimaryDocument == value)
                return;

            if (selectedPrimaryDocument != null)
                selectedPrimaryDocument.IsSelected = false;

            selectedPrimaryDocument = value;

            if (selectedPrimaryDocument != null)
                selectedPrimaryDocument.IsSelected = true;

            OnPropertyChanged(nameof(SelectedPrimaryDocument));
        }

        private void DoSetSelectedSecondaryDocument(BaseDocumentViewModel value)
        {
            if (selectedSecondaryDocument == value)
                return;

            if (selectedSecondaryDocument != null)
                selectedSecondaryDocument.IsSelected = false;

            selectedSecondaryDocument = value;

            if (selectedSecondaryDocument != null)
                selectedSecondaryDocument.IsSelected = true;

            OnPropertyChanged(nameof(SelectedSecondaryDocument));
        }

        private void SetSelectedPrimaryDocument(BaseDocumentViewModel value)
        {
            System.Diagnostics.Debug.WriteLine($"DocumentsManager.SetSelectedPrimaryDocument, ViewModel: {value?.FileName ?? "(none)"}");

            if (value == selectedPrimaryDocument)
                return;
            
            DoSetSelectedPrimaryDocument(value);

            UpdateActiveDocumentToSelected();
        }

        private void SetSelectedSecondaryDocument(BaseDocumentViewModel value)
        {
            System.Diagnostics.Debug.WriteLine($"DocumentsManager.SetSelectedSecondaryDocument, ViewModel: {value?.FileName ?? "(none)"}");

            if (value == selectedSecondaryDocument)
                return;

            DoSetSelectedSecondaryDocument(value);

            UpdateActiveDocumentToSelected();
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

                DoSetSelectedPrimaryDocument(value);
                DoSetActiveDocument(value);
            }
            else if (secondaryDocuments.Contains(value))
            {
                if (activeDocumentTab != DocumentTabKind.Secondary)
                {
                    activeDocumentTab = DocumentTabKind.Secondary;
                    OnPropertyChanged(nameof(ActiveDocumentTab));
                }

                DoSetSelectedSecondaryDocument(value);
                DoSetActiveDocument(value);
            }
            else
                throw new ArgumentException("Document not found!", nameof(value));
        }

        private void DoSetActiveDocument(BaseDocumentViewModel value)
        {
            if (activeDocument == value)
                return;

            if (activeDocument != null)
                activeDocument.IsActive = false;

            activeDocument = value;

            if (activeDocument != null)
                activeDocument.IsActive = true;

            OnPropertyChanged(nameof(ActiveDocument));
        }

        private void SetActiveDocumentTab(DocumentTabKind value)
        {
            if (value == activeDocumentTab)
                return;

            activeDocumentTab = value;
            OnPropertyChanged(nameof(ActiveDocumentTab));

            UpdateActiveDocumentToSelected();
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

                DoSetActiveDocument(selectedPrimaryDocument);
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

                if (index >= 0 && index < collection.Count)
                    selectedCollectionItem = collection[index];
                else
                    selectedCollectionItem = null;

                OnPropertyChanged(selectedPropertyName);

                UpdateActiveDocumentToSelected();
            }

            AutoAdjustSecondaryTabVisibility();
        }

        private void InternalRemoveDocumentAt(int index,
            TabDocumentCollection<BaseDocumentViewModel> collection,
            ref BaseDocumentViewModel selectedCollectionItem,
            string selectedPropertyName)
        {
            if (index < 0 || index >= collection.Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            var document = collection[index];
            collection.RemoveAt(index);

            if (selectedCollectionItem == document)
            {
                if (index >= collection.Count)
                    index = collection.Count - 1;

                if (index > 0 && index < collection.Count)
                    selectedCollectionItem = collection[index];
                else
                    selectedCollectionItem = null;

                OnPropertyChanged(selectedPropertyName);

                UpdateActiveDocumentToSelected();
            }

            AutoAdjustSecondaryTabVisibility();
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

                UpdateActiveDocumentToSelected();
            }
        }

        private void UpdateActiveDocumentToSelected()
        {
            if (activeDocumentTab == DocumentTabKind.Primary)
            {
                DoSetActiveDocument(selectedPrimaryDocument);
            }
            else if (activeDocumentTab == DocumentTabKind.Secondary)
            {
                DoSetActiveDocument(selectedSecondaryDocument);
            }
            else
                throw new InvalidOperationException("Unsupported active document tab!");
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
            {
                var selectedDocument = selectedPrimaryDocument;

                InternalRemoveDocumentFrom(document,
                    primaryDocuments,
                    ref selectedDocument,
                    nameof(SelectedPrimaryDocument));

                DoSetSelectedPrimaryDocument(selectedDocument);
            }
            else if (secondaryDocuments.Contains(document))
            {
                var selectedDocument = selectedSecondaryDocument;

                InternalRemoveDocumentFrom(document,
                    secondaryDocuments,
                    ref selectedDocument,
                    nameof(SelectedSecondaryDocument));

                DoSetSelectedSecondaryDocument(selectedDocument);
            }
            else
                throw new ArgumentException(nameof(document));
        }

        public void RemoveDocument(DocumentTabKind documentTabKind, BaseDocumentViewModel document)
        {
            switch (documentTabKind)
            {
                case DocumentTabKind.Primary:
                    {
                        var selectedDocument = selectedPrimaryDocument;

                        InternalRemoveDocumentFrom(document,
                            primaryDocuments,
                            ref selectedDocument,
                            nameof(SelectedPrimaryDocument));

                        SetSelectedPrimaryDocument(selectedDocument);
                        break;
                    }
                case DocumentTabKind.Secondary:
                    {
                        var selectedDocument = selectedSecondaryDocument;

                        InternalRemoveDocumentFrom(document,
                            secondaryDocuments,
                            ref selectedDocument,
                            nameof(SelectedSecondaryDocument));

                        SetSelectedSecondaryDocument(selectedDocument);
                        break;
                    }
                default:
                    throw new InvalidEnumArgumentException("Unsupported document tab kind!");
            }
        }

        public void RemoveDocumentAt(DocumentTabKind documentTabKind, int index)
        {
            switch (documentTabKind)
            {
                case DocumentTabKind.Primary:
                    {
                        var selectedDocument = selectedPrimaryDocument;

                        InternalRemoveDocumentAt(index,
                            primaryDocuments,
                            ref selectedDocument,
                            nameof(SelectedPrimaryDocument));

                        SetSelectedPrimaryDocument(selectedDocument);
                        break;
                    }
                case DocumentTabKind.Secondary:
                    {
                        var selectedDocument = selectedSecondaryDocument;

                        InternalRemoveDocumentAt(index,
                            secondaryDocuments,
                            ref selectedDocument,
                            nameof(SelectedSecondaryDocument));

                        SetSelectedSecondaryDocument(selectedDocument);
                        break;
                    }
                default:
                    throw new InvalidEnumArgumentException("Unsupported document tab kind!");
            }
        }

        public void AddDocument(BaseDocumentViewModel document, DocumentTabKind documentTabKind)
        {
            switch (documentTabKind)
            {
                case DocumentTabKind.Primary:
                    {
                        var selectedDocument = selectedPrimaryDocument;

                        InternalAddDocumentTo(document, primaryDocuments, ref selectedDocument, nameof(SelectedPrimaryDocument));

                        SetSelectedPrimaryDocument(selectedDocument);
                        break;
                    }
                case DocumentTabKind.Secondary:
                    {
                        var selectedDocument = selectedSecondaryDocument;

                        InternalAddDocumentTo(document, secondaryDocuments, ref selectedDocument, nameof(SelectedSecondaryDocument));

                        SetSelectedSecondaryDocument(selectedDocument);
                        AutoAdjustSecondaryTabVisibility();
                        break;
                    }
                default:
                    throw new InvalidEnumArgumentException("Unsupported document tab kind!");
            }
        }

        private void AutoAdjustSecondaryTabVisibility()
        {
            if (!showSecondaryDocumentTab && secondaryDocuments.Count > 0)
            {
                SetShowSecondaryDocumentsTab(true);
            }
            else if (showSecondaryDocumentTab && secondaryDocuments.Count == 0)
            {
                SetShowSecondaryDocumentsTab(false);
                SetActiveDocumentTab(DocumentTabKind.Primary);
            }
        }

        public void InsertDocument(int index, BaseDocumentViewModel document, DocumentTabKind documentTabKind)
        {
            switch (documentTabKind)
            {
                case DocumentTabKind.Primary:
                    {
                        var selectedDocument = selectedPrimaryDocument;

                        InternalInsertDocumentTo(index, document, primaryDocuments, ref selectedDocument, nameof(SelectedPrimaryDocument));

                        SetSelectedPrimaryDocument(selectedDocument);
                        break;
                    }
                case DocumentTabKind.Secondary:
                    {
                        var selectedDocument = selectedSecondaryDocument;

                        InternalInsertDocumentTo(index, document, secondaryDocuments, ref selectedDocument, nameof(SelectedSecondaryDocument));

                        SetSelectedSecondaryDocument(selectedDocument);

                        if (!showSecondaryDocumentTab)
                        {
                            showSecondaryDocumentTab = true;
                            OnPropertyChanged(nameof(showSecondaryDocumentTab));
                        }
                        break;
                    }
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
                    destinationDocuments = primaryDocuments;
                    break;
                case DocumentTabKind.Secondary:
                    destinationDocuments = secondaryDocuments;
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

            AutoAdjustSecondaryTabVisibility();

            SetActiveDocument(documentViewModel);
        }

        public DocumentTabKind GetTabOf(BaseDocumentViewModel document)
        {
            if (primaryDocuments.Contains(document))
                return DocumentTabKind.Primary;
            else if (secondaryDocuments.Contains(document))
                return DocumentTabKind.Secondary;
            else
                throw new ArgumentException(nameof(document));
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

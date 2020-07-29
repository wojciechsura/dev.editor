using Dev.Editor.BusinessLogic.Models.Configuration.Search;
using Dev.Editor.BusinessLogic.Models.Dialogs;
using Dev.Editor.BusinessLogic.Models.Events;
using Dev.Editor.BusinessLogic.Models.Search;
using Dev.Editor.BusinessLogic.Services.Config;
using Dev.Editor.BusinessLogic.Services.Dialogs;
using Dev.Editor.BusinessLogic.Services.EventBus;
using Dev.Editor.BusinessLogic.Services.Messaging;
using Dev.Editor.BusinessLogic.Services.SearchEncoder;
using Dev.Editor.BusinessLogic.Types.Search;
using Dev.Editor.BusinessLogic.ViewModels.Base;
using Dev.Editor.Common.Commands;
using Dev.Editor.Common.Conditions;
using Dev.Editor.Common.Tools;
using Dev.Editor.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dev.Editor.BusinessLogic.ViewModels.Search
{
    public class SearchReplaceWindowViewModel : BaseViewModel, IEventListener<ApplicationShutdownEvent>, IEventListener<StoredSearchesChangedEvent>
    {
        private const int LastSearchCount = 10;

        // Private fields -----------------------------------------------------

        private readonly ISearchReplaceWindowAccess access;
        private readonly IConfigurationService configurationService;
        private readonly IEventBus eventBus;
        private readonly IMessagingService messagingService;

        private readonly ISearchHost searchHost;
        private readonly ISearchEncoderService searchEncoderService;
        private readonly IDialogService dialogService;

        private readonly Condition searchExpressionValidCondition;
        private readonly Condition storedSearchSelectedCondition;

        private readonly ObservableCollection<StoredSearchViewModel> storedSearches = new ObservableCollection<StoredSearchViewModel>();
        private StoredSearchViewModel selectedStoredSearch;

        private bool caseSensitive;
        private bool modelUpdatedSinceLastSearch = true;
        private string replace;
        private string search;
        private bool searchBackwards;
        private bool inSelection;
        private SearchMode searchMode;
        private SearchReplaceModel searchReplaceModel;
        private bool selectionAvailable;
        private bool wholeWordsOnly;
        private SearchReplaceOperation currentOperation;
        private bool showReplaceSummary;
        private bool storedSearchPanelVisible;
        private string location;
        private string fileMask;

        // Private methods ----------------------------------------------------

        private void BuildStoredSearches()
        {
            storedSearches.Clear();
            configurationService.Configuration.SearchConfig.StoredSearchReplaces
                .Select(sr => new StoredSearchViewModel(sr))
                .OrderBy(sr => sr.SearchName)
                .ToList()
                .ForEach(srvm => storedSearches.Add(srvm));
        }

        private void DoClose()
        {
            access.Close();
        }

        private void DoSaveSearch()
        {
            var model = new NameDialogModel("", Strings.Dialog_StoredSearch_Title, Strings.Dialog_StoredSearch_Title);
            (bool result, NameDialogResult data) = dialogService.ShowChooseNameDialog(model);
            if (result)
            {
                var newEntry = new StoredSearchReplace();
                newEntry.IsCaseSensitive.Value = CaseSensitive;
                newEntry.IsSearchBackwards.Value = SearchBackwards;
                newEntry.IsWholeWordsOnly.Value = WholeWordsOnly;
                newEntry.Operation.Value = CurrentOperation;
                newEntry.Replace.Value = Replace;
                newEntry.Search.Value = Search;
                newEntry.SearchMode.Value = SearchMode;
                newEntry.SearchName.Value = data.Name;
                newEntry.ShowReplaceSummary.Value = ShowReplaceSummary;

                configurationService.Configuration.SearchConfig.StoredSearchReplaces.Add(newEntry);
                configurationService.Save();

                // Insert new viewmodel to avoid reloading whole list

                var viewmodel = new StoredSearchViewModel(newEntry);
                int i = 0;
                while (i < storedSearches.Count && String.Compare(storedSearches[i].SearchName, viewmodel.SearchName) < 0)
                    i++;
                storedSearches.Insert(i, viewmodel);

                // Notify interested parties about change

                eventBus.Send(this, new StoredSearchesChangedEvent());
            }
        }

        private void DoDeleteSearch()
        {
            if (messagingService.AskYesNo(String.Format(Strings.Message_StoredSearchDeleteConfirmation, SelectedStoredSearch.SearchName)) == true)
            {
                // Delete stored search

                configurationService.Configuration.SearchConfig.StoredSearchReplaces.Remove(SelectedStoredSearch.StoredSearch);
                configurationService.Save();

                // Remove viewmodel to avoid reloading whole list

                storedSearches.Remove(SelectedStoredSearch);
                SelectedStoredSearch = null;

                // Notify interested parties about change

                eventBus.Send(this, new StoredSearchesChangedEvent());
            }
        }

        private void StoreLastSearchReplaceIfNeeded()
        {
            if (modelUpdatedSinceLastSearch)
            {
                StoreLastSearchReplace();
            }

            modelUpdatedSinceLastSearch = false;
        }

        private void DoFindNext()
        {
            StoreLastSearchReplaceIfNeeded();

            if (!searchReplaceModel.SearchPerformed)
            {
                // If this is a new search, update search/replace segment
                // to define the search area.

                if (inSelection)
                    searchHost.SetFindReplaceSegmentToSelection(searchReplaceModel.SearchBackwards);
                else
                    searchHost.ClearFindReplaceSegment();
            }

            searchHost.FindNext(searchReplaceModel);
        }

        private void DoFindInFiles()
        {
            StoreLastSearchReplaceIfNeeded();

            // Validate location

            if (!Directory.Exists(location))
            {
                messagingService.ShowError(Strings.Message_LocationInvalid);
                return;
            }

            searchHost.FindInFiles(searchReplaceModel);

            // Find in files closes the search window, because progress window appears instead
            access.Close();
        }


        private void DoCountOccurrences()
        {
            try
            {
                StoreLastSearchReplaceIfNeeded();
                searchHost.CountOccurrences(searchReplaceModel);
            }
            catch
            {
                messagingService.ShowError(Strings.Message_InvalidSearchPattern);
                return;
            }
        }

        private void DoPickLocation()
        {
            (bool result, string newLocation) = dialogService.ShowChooseFolderDialog(location);

            if (result)
                Location = newLocation;
        }

        private void DoReplace()
        {
            try
            {
                StoreLastSearchReplaceIfNeeded();
                searchHost.Replace(searchReplaceModel);
            }
            catch
            {
                messagingService.ShowError(Strings.Message_InvalidSearchPattern);
                return;
            }
        }

        private void DoReplaceAll()
        {
            try
            {
                StoreLastSearchReplaceIfNeeded();
                searchHost.ReplaceAll(searchReplaceModel);
            }
            catch
            {
                messagingService.ShowError(Strings.Message_InvalidSearchPattern);
                return;
            }
        }

        private void HandleSearchReplaceParamsChanged()
        {
            UpdateModel();
        }

        private void HandleSelectedStoredSearchChanged()
        {
            storedSearchSelectedCondition.Value = SelectedStoredSearch != null;
        }

        private void HandleSelectionAvailableChanged(object sender, ValueChangedEventArgs e)
        {
            SelectionAvailable = searchHost.SelectionAvailableCondition.GetValue();
            if (!SelectionAvailable)
                InSelection = false;
        }

        private void StoreLastSearchReplace()
        {
            StoreLastString(Search, LastSearches);
            StoreLastString(Replace, LastReplaces);
        }

        private void StoreLastString(string str, ObservableCollection<string> list)
        {
            var index = list.IndexOf(str);
            if (index >= 0)
            {
                if (index > 0)
                    list.Move(index, 0);
            }                
            else
            {
                list.Insert(0, str);
                while (list.Count > LastSearchCount)
                    list.RemoveAt(list.Count - 1);
            }
        }

        private SearchReplaceDescription BuildSearchDescription()
        {
            return new SearchReplaceDescription(
                Search,
                Replace,
                currentOperation,
                SearchMode,
                CaseSensitive,
                InSelection,
                SearchBackwards,
                WholeWordsOnly,
                ShowReplaceSummary,
                Location,
                FileMask);
        }

        private void UpdateModel()
        {
            var searchDescription = BuildSearchDescription();

            try
            {
                searchReplaceModel = searchEncoderService.SearchDescriptionToModel(searchDescription);

                searchExpressionValidCondition.Value = true;
            }
            catch (InvalidSearchExpressionException)
            {
                searchExpressionValidCondition.Value = false;
            }

            modelUpdatedSinceLastSearch = true;
        }

        // IEventListener<ApplicationShutdownEvent> implementation ------------

        void IEventListener<ApplicationShutdownEvent>.Receive(ApplicationShutdownEvent @event)
        {
            // Store last search/replaces

            configurationService.Configuration.SearchConfig.LastSearchTexts.Clear();
            LastSearches.ForEach(ls =>
            {
                var searchText = new SearchText();
                searchText.Text.Value = ls;
                configurationService.Configuration.SearchConfig.LastSearchTexts.Add(searchText);
            });

            configurationService.Configuration.SearchConfig.LastReplaceTexts.Clear();
            LastReplaces.ForEach(lr =>
            {
                var replaceText = new ReplaceText();
                replaceText.Text.Value = lr;
                configurationService.Configuration.SearchConfig.LastReplaceTexts.Add(replaceText);
            });

            // Store recent search settings

            var recentSearchSettings = configurationService.Configuration.SearchConfig.RecentSearchSettings;

            recentSearchSettings.CaseSensitive.Value = caseSensitive;
            recentSearchSettings.ReplaceAllInSelection.Value = inSelection;
            recentSearchSettings.SearchBackwards.Value = searchBackwards;
            recentSearchSettings.SearchMode.Value = searchMode;
            recentSearchSettings.ShowReplaceSummary.Value = showReplaceSummary;
            recentSearchSettings.WholeWordsOnly.Value = wholeWordsOnly;
        }

        // IEventListener<StoredSearchesChangedEvent> implementation ----------

        void IEventListener<StoredSearchesChangedEvent>.Receive(StoredSearchesChangedEvent @event)
        {
            BuildStoredSearches();
        }

        // Public methods -----------------------------------------------------

        public SearchReplaceWindowViewModel(ISearchHost searchHost,
            ISearchReplaceWindowAccess access,
            IMessagingService messagingService,
            IConfigurationService configurationService,
            IEventBus eventBus,
            ISearchEncoderService searchEncoderService,
            IDialogService dialogService)
        {
            this.searchHost = searchHost;
            this.access = access;
            this.messagingService = messagingService;
            this.configurationService = configurationService;
            this.eventBus = eventBus;
            this.searchEncoderService = searchEncoderService;
            this.dialogService = dialogService;

            this.fileMask = "*.*";
            this.location = "";

            eventBus.Register((IEventListener<ApplicationShutdownEvent>)this);
            eventBus.Register((IEventListener<StoredSearchesChangedEvent>)this);

            var recentSearchSettings = configurationService.Configuration.SearchConfig.RecentSearchSettings;

            caseSensitive = recentSearchSettings.CaseSensitive.Value;
            inSelection = recentSearchSettings.ReplaceAllInSelection.Value;
            searchBackwards = recentSearchSettings.SearchBackwards.Value;
            searchMode = recentSearchSettings.SearchMode.Value;
            showReplaceSummary = recentSearchSettings.ShowReplaceSummary.Value;
            wholeWordsOnly = recentSearchSettings.WholeWordsOnly.Value;

            storedSearchPanelVisible = false;
            BuildStoredSearches();

            LastSearches = new ObservableCollection<string>();
            configurationService.Configuration.SearchConfig.LastSearchTexts.ForEach(st => LastSearches.Add(st.Text.Value));
            if (LastSearches.Any())
                search = LastSearches.First();
            else
                search = null;

            LastReplaces = new ObservableCollection<string>();
            configurationService.Configuration.SearchConfig.LastSearchTexts.ForEach(rt => LastReplaces.Add(rt.Text.Value));
            if (LastReplaces.Any())
                replace = LastReplaces.First();
            else
                replace = null;

            searchExpressionValidCondition = new Condition(false);
            storedSearchSelectedCondition = new Condition(SelectedStoredSearch != null);

            selectionAvailable = searchHost.SelectionAvailableCondition.GetValue();
            searchHost.SelectionAvailableCondition.ValueChanged += HandleSelectionAvailableChanged;

            FindNextCommand = new AppCommand(obj => DoFindNext(), searchHost.CanSearchCondition & searchExpressionValidCondition);
            FindInFilesCommand = new AppCommand(obj => DoFindInFiles(), searchExpressionValidCondition);
            CountOccurrencesCommand = new AppCommand(obj => DoCountOccurrences(), searchHost.CanSearchCondition & searchExpressionValidCondition);
            ReplaceCommand = new AppCommand(obj => DoReplace(), searchHost.CanSearchCondition & searchExpressionValidCondition);
            ReplaceAllCommand = new AppCommand(obj => DoReplaceAll(), searchHost.CanSearchCondition & searchExpressionValidCondition);
            CloseCommand = new AppCommand(obj => DoClose());
            SaveSearchCommand = new AppCommand(obj => DoSaveSearch());
            DeleteSearchCommand = new AppCommand(obj => DoDeleteSearch(), storedSearchSelectedCondition);
            PickLocationCommand = new AppCommand(obj => DoPickLocation());

            UpdateModel();
        }

        public void ShowReplace()
        {
            inSelection = searchHost.SelectionAvailableCondition.GetValue();

            CurrentOperation = SearchReplaceOperation.Replace;
            
            access.FocusReplace();
            access.ShowAndFocus();
        }

        public void ShowSearch()
        {
            CurrentOperation = SearchReplaceOperation.Search;
            
            access.FocusFindInFiles();
            access.ShowAndFocus();
        }

        public void ShowFindInFiles()
        {
            CurrentOperation = SearchReplaceOperation.FindInFiles;

            access.FocusSearch();
            access.ShowAndFocus();
        }

        public void StoredSearchSelected()
        {
            if (SelectedStoredSearch != null)
            {
                CaseSensitive = SelectedStoredSearch.StoredSearch.IsCaseSensitive.Value;
                SearchBackwards = SelectedStoredSearch.StoredSearch.IsSearchBackwards.Value;
                WholeWordsOnly = SelectedStoredSearch.StoredSearch.IsWholeWordsOnly.Value;
                CurrentOperation = SelectedStoredSearch.StoredSearch.Operation.Value;
                Replace = SelectedStoredSearch.StoredSearch.Replace.Value;
                Search = SelectedStoredSearch.StoredSearch.Search.Value;
                SearchMode = SelectedStoredSearch.StoredSearch.SearchMode.Value;
                ShowReplaceSummary = SelectedStoredSearch.StoredSearch.ShowReplaceSummary.Value;
            }
        }

        public void NotifyClosed()
        {
            searchHost.ClearFindReplaceSegment();
        }

        // Public properties --------------------------------------------------

        public bool CaseSensitive
        {
            get => caseSensitive;
            set => Set(ref caseSensitive, () => CaseSensitive, value, HandleSearchReplaceParamsChanged);
        }

        public ICommand CloseCommand { get; }

        public ICommand FindNextCommand { get; }

        public ICommand CountOccurrencesCommand { get; }

        public ObservableCollection<string> LastReplaces { get; }

        public ObservableCollection<string> LastSearches { get; }

        public string Replace
        {
            get => replace;
            set => Set(ref replace, () => Replace, value, HandleSearchReplaceParamsChanged);
        }

        public ICommand ReplaceAllCommand { get; }

        public bool InSelection
        {
            get => inSelection;
            set => Set(ref inSelection, () => InSelection, value, HandleSearchReplaceParamsChanged);
        }

        public ICommand ReplaceCommand { get; }

        public ICommand SaveSearchCommand { get; }

        public ICommand DeleteSearchCommand { get; }

        public ICommand FindInFilesCommand { get; }

        public ICommand PickLocationCommand { get; }

        public string Location
        {
            get => location;
            set => Set(ref location, () => Location, value, HandleSearchReplaceParamsChanged);
        }

        public string FileMask
        {
            get => fileMask;
            set => Set(ref fileMask, () => FileMask, value, HandleSearchReplaceParamsChanged);
        }

        public string Search
        {
            get => search;
            set => Set(ref search, () => Search, value, HandleSearchReplaceParamsChanged);
        }
        
        public bool SearchBackwards
        {
            get => searchBackwards;
            set => Set(ref searchBackwards, () => SearchBackwards, value, HandleSearchReplaceParamsChanged);
        }

        public SearchMode SearchMode
        {
            get => searchMode;
            set => Set(ref searchMode, () => SearchMode, value, HandleSearchReplaceParamsChanged);
        }

        public bool SelectionAvailable
        {
            get => selectionAvailable;
            set => Set(ref selectionAvailable, () => SelectionAvailable, value, HandleSearchReplaceParamsChanged);
        }

        public bool WholeWordsOnly
        {
            get => wholeWordsOnly;
            set => Set(ref wholeWordsOnly, () => WholeWordsOnly, value, HandleSearchReplaceParamsChanged);
        }

        public bool ShowReplaceSummary
        {
            get => showReplaceSummary;
            set => Set(ref showReplaceSummary, () => ShowReplaceSummary, value);
        }

        public SearchReplaceOperation CurrentOperation
        {
            get => currentOperation;
            set => Set(ref currentOperation, () => CurrentOperation, value);
        }

        public bool StoredSearchPanelVisible 
        {
            get => storedSearchPanelVisible;
            set => Set(ref storedSearchPanelVisible, () => StoredSearchPanelVisible, value);
        }

        public ObservableCollection<StoredSearchViewModel> StoredSearches => storedSearches;

        public StoredSearchViewModel SelectedStoredSearch
        {
            get => selectedStoredSearch;
            set => Set(ref selectedStoredSearch, () => SelectedStoredSearch, value, HandleSelectedStoredSearchChanged);
        }        
    }
}

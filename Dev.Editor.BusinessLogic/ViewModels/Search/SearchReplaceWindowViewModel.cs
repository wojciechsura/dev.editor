using Dev.Editor.BusinessLogic.Models.Configuration.Search;
using Dev.Editor.BusinessLogic.Models.Events;
using Dev.Editor.BusinessLogic.Models.Search;
using Dev.Editor.BusinessLogic.Services.Config;
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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dev.Editor.BusinessLogic.ViewModels.Search
{
    public class SearchReplaceWindowViewModel : BaseViewModel, IEventListener<ApplicationShutdownEvent>
    {
        private const int LastSearchCount = 10;

        // Private fields -----------------------------------------------------

        private readonly ISearchReplaceWindowAccess access;
        private readonly IConfigurationService configurationService;
        private readonly IEventBus eventBus;
        private readonly IMessagingService messagingService;
        private readonly ISearchHost searchHost;
        private readonly ISearchEncoderService searchEncoderService;

        private readonly Condition searchExpressionValidCondition;

        private bool caseSensitive;
        private bool modelUpdatedSinceLastSearch = true;
        private string replace;
        private bool replaceAllInSelection;
        private string search;
        private bool searchBackwards;
        private SearchMode searchMode;
        private SearchReplaceModel searchReplaceModel;
        private bool selectionAvailable;
        private bool wholeWordsOnly;
        private SearchReplaceOperation currentOperation;
        private bool showReplaceSummary;

        // Private methods ----------------------------------------------------

        private void DoClose() => access.Close();

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
            try
            {
                StoreLastSearchReplaceIfNeeded();
                searchHost.FindNext(searchReplaceModel);
            }
            catch
            {
                messagingService.ShowError(Strings.Message_InvalidSearchPattern);
                return;
            }
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

        private void HandleSelectionAvailableChanged(object sender, ValueChangedEventArgs e)
        {
            SelectionAvailable = searchHost.SelectionAvailableCondition.GetValue();
            if (!SelectionAvailable)
                ReplaceAllInSelection = false;
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
                ReplaceAllInSelection,
                SearchBackwards,
                WholeWordsOnly,
                ShowReplaceSummary);
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
        }

        // Public methods -----------------------------------------------------

        public SearchReplaceWindowViewModel(ISearchHost searchHost,
            ISearchReplaceWindowAccess access,
            IMessagingService messagingService,
            IConfigurationService configurationService,
            IEventBus eventBus,
            ISearchEncoderService searchEncoderService)
        {
            this.searchHost = searchHost;
            this.access = access;
            this.messagingService = messagingService;
            this.configurationService = configurationService;
            this.eventBus = eventBus;
            this.searchEncoderService = searchEncoderService;

            eventBus.Register((IEventListener<ApplicationShutdownEvent>)this);

            caseSensitive = false;
            wholeWordsOnly = false;
            searchMode = SearchMode.Normal;

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

            selectionAvailable = searchHost.SelectionAvailableCondition.GetValue();
            searchHost.SelectionAvailableCondition.ValueChanged += HandleSelectionAvailableChanged;

            FindNextCommand = new AppCommand(obj => DoFindNext(), searchHost.CanSearchCondition & searchExpressionValidCondition);
            ReplaceCommand = new AppCommand(obj => DoReplace(), searchHost.CanSearchCondition & searchExpressionValidCondition);
            ReplaceAllCommand = new AppCommand(obj => DoReplaceAll(), searchHost.CanSearchCondition & searchExpressionValidCondition);
            CloseCommand = new AppCommand(obj => DoClose());

            UpdateModel();
        }

        public void ShowReplace()
        {
            replaceAllInSelection = searchHost.SelectionAvailableCondition.GetValue();

            CurrentOperation = SearchReplaceOperation.Replace;
            
            access.FocusReplace();
            access.ShowAndFocus();
        }

        public void ShowSearch()
        {
            CurrentOperation = SearchReplaceOperation.Search;
            
            access.FocusSearch();
            access.ShowAndFocus();
        }

        // Public properties --------------------------------------------------

        public bool CaseSensitive
        {
            get => caseSensitive;
            set => Set(ref caseSensitive, () => CaseSensitive, value, HandleSearchReplaceParamsChanged);
        }

        public ICommand CloseCommand { get; }

        public ICommand FindNextCommand { get; }

        public ObservableCollection<string> LastReplaces { get; }

        public ObservableCollection<string> LastSearches { get; }

        public string Replace
        {
            get => replace;
            set => Set(ref replace, () => Replace, value, HandleSearchReplaceParamsChanged);
        }

        public ICommand ReplaceAllCommand { get; }

        public bool ReplaceAllInSelection
        {
            get => replaceAllInSelection;
            set => Set(ref replaceAllInSelection, () => ReplaceAllInSelection, value, HandleSearchReplaceParamsChanged);
        }

        public ICommand ReplaceCommand { get; }

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
    }
}

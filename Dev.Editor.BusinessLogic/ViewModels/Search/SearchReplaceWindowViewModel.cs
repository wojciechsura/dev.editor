using Dev.Editor.BusinessLogic.Models.Configuration.Search;
using Dev.Editor.BusinessLogic.Models.Events;
using Dev.Editor.BusinessLogic.Models.Search;
using Dev.Editor.BusinessLogic.Services.Config;
using Dev.Editor.BusinessLogic.Services.EventBus;
using Dev.Editor.BusinessLogic.Services.Messaging;
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
        private const string EolFixString = "\r?";
        private const int LastSearchCount = 10;

        // Private fields -----------------------------------------------------

        private readonly ISearchReplaceWindowAccess access;
        private readonly IConfigurationService configurationService;
        private readonly IEventBus eventBus;
        private readonly IMessagingService messagingService;
        private readonly ISearchHost searchHost;
        private readonly Condition searchRegexValidCondition;

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

        // Private methods ----------------------------------------------------

        private void DoClose() => access.Close();

        private void DoFindNext()
        {
            try
            {
                if (modelUpdatedSinceLastSearch)
                {
                    StoreLastSearchReplace();
                }

                modelUpdatedSinceLastSearch = false;
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
                searchHost.ReplaceAll(searchReplaceModel);
            }
            catch
            {
                messagingService.ShowError(Strings.Message_InvalidSearchPattern);
                return;
            }
        }

        /// <summary>
        /// Replaces $ signs in the pattern with \r?$
        /// Takes into account, that $ signs in the pattern
        /// may be escaped. Handles properly situations, that
        /// might effect in false positives (such as "\$" in "\\$").
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        private string FixDotNetEoLRegex(string pattern)
        {
            int startIndex = 0;

            while (startIndex < pattern.Length)
            {
                var res = pattern.IndexOf('$', startIndex);
                if (res == -1)
                {
                    // No more $'s
                    break;
                }

                // Check escaping
                int backslashCount = 0;
                for (int i = res - 1; i >= 0; i--)
                {
                    if (pattern[i] == '\\')
                        backslashCount++;
                    else
                        break;
                }

                if (backslashCount % 2 == 1)
                {
                    // Odd count of backslashes - $ is escaped
                    startIndex = res + 1;
                    continue;
                }

                // Add \r? before the $ sign
                pattern.Insert(res, EolFixString);
                res += EolFixString.Length;

                startIndex = res + 1;
            }

            return pattern;
        }

        private string GetReplaceText(string replace)
        {
            replace = replace ?? String.Empty;

            switch (searchMode)
            {
                case SearchMode.Normal:
                    return replace;
                case SearchMode.Extended:
                    return replace.Unescape();
                case SearchMode.RegularExpressions:
                    return replace;
                default:
                    throw new InvalidOperationException("Unsupported search mode!");
            }
        }

        private Regex GetSearchRegex(string textToFind)
        {
            RegexOptions options = RegexOptions.None;
            if (searchBackwards)
                options |= RegexOptions.RightToLeft;
            if (!caseSensitive)
                options |= RegexOptions.IgnoreCase;
            options |= RegexOptions.Multiline;

            switch (searchMode)
            {
                case SearchMode.RegularExpressions:
                    {
                        string pattern = textToFind;

                        // See: https://docs.microsoft.com/en-us/dotnet/standard/base-types/anchors-in-regular-expressions#end-of-string-or-line-
                        pattern = FixDotNetEoLRegex(pattern);

                        return new Regex(pattern, options);
                    }
                case SearchMode.Extended:
                    {
                        string pattern = Regex.Escape(textToFind.Unescape());

                        if (wholeWordsOnly)
                            pattern = "\\b" + pattern + "\\b";
                        return new Regex(pattern, options);
                    }
                case SearchMode.Normal:
                    {
                        string pattern = Regex.Escape(textToFind);
                        if (wholeWordsOnly)
                            pattern = "\\b" + pattern + "\\b";
                        return new Regex(pattern, options);
                    }

                default:
                    throw new InvalidOperationException("Unsupported search mode!");
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
        private void UpdateModel()
        {
            searchReplaceModel = null;

            Regex searchRegex;

            try
            {
                searchRegex = GetSearchRegex(search);
                searchRegexValidCondition.Value = true;
            }
            catch
            {
                searchRegexValidCondition.Value = false;
                return;
            }

            string replaceText = GetReplaceText(replace);

            searchReplaceModel = new SearchReplaceModel(searchRegex, replaceText, searchBackwards, searchMode == SearchMode.RegularExpressions, replaceAllInSelection);
            modelUpdatedSinceLastSearch = true;
        }
        // IEventListener<ApplicationShutdownEvent> implementation ------------

        public SearchReplaceWindowViewModel(ISearchHost searchHost,
            ISearchReplaceWindowAccess access,
            IMessagingService messagingService,
            IConfigurationService configurationService,
            IEventBus eventBus)
        {
            this.searchHost = searchHost;
            this.access = access;
            this.messagingService = messagingService;
            this.configurationService = configurationService;
            this.eventBus = eventBus;

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

            selectionAvailable = searchHost.SelectionAvailableCondition.GetValue();
            searchHost.SelectionAvailableCondition.ValueChanged += HandleSelectionAvailableChanged;

            searchRegexValidCondition = new Condition(true);

            FindNextCommand = new AppCommand(obj => DoFindNext(), searchHost.CanSearchCondition & searchRegexValidCondition);
            ReplaceCommand = new AppCommand(obj => DoReplace(), searchHost.CanSearchCondition & searchRegexValidCondition);
            ReplaceAllCommand = new AppCommand(obj => DoReplaceAll(), searchHost.CanSearchCondition & searchRegexValidCondition);
            CloseCommand = new AppCommand(obj => DoClose());

            UpdateModel();
        }

        public void ShowReplace()
        {
            replaceAllInSelection = searchHost.SelectionAvailableCondition.GetValue();

            access.ChooseReplaceTab();
            access.ShowAndFocus();
        }

        // Public methods -----------------------------------------------------
        public void ShowSearch()
        {
            access.ChooseSearchTab();
            access.ShowAndFocus();
        }

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
    }
}

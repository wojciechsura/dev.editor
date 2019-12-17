using Dev.Editor.BusinessLogic.Models.Search;
using Dev.Editor.BusinessLogic.Services.Messaging;
using Dev.Editor.BusinessLogic.Types.Search;
using Dev.Editor.BusinessLogic.ViewModels.Base;
using Dev.Editor.Common.Commands;
using Dev.Editor.Common.Conditions;
using Dev.Editor.Common.Tools;
using Dev.Editor.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dev.Editor.BusinessLogic.ViewModels.Search
{
    public class SearchReplaceWindowViewModel : BaseViewModel
    {
        // Private fields -----------------------------------------------------

        private readonly ISearchHost searchHost;
        private readonly ISearchReplaceWindowAccess access;
        private readonly IMessagingService messagingService;

        private string search;
        private string replace;
        private bool caseSensitive;
        private bool wholeWordsOnly;
        private bool searchBackwards;
        private SearchMode searchMode;

        private bool selectionAvailable;
        private bool replaceAllInSelection;

        private readonly Condition searchRegexValidCondition;

        private SearchReplaceModel searchReplaceModel;

        // Private methods ----------------------------------------------------

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
                        pattern = Regex.Replace(pattern, @"(?<!\\)\$", @"\r?$");

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
        }

        private void DoClose() => access.Close();

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

        private void DoFindNext()
        {
            try
            {
                searchHost.FindNext(searchReplaceModel);
            }
            catch
            {
                messagingService.ShowError(Strings.Message_InvalidSearchPattern);
                return;
            }
        }

        private void HandleSelectionAvailableChanged(object sender, ValueChangedEventArgs e)
        {
            SelectionAvailable = searchHost.SelectionAvailableCondition.GetValue();
            if (!SelectionAvailable)
                ReplaceAllInSelection = false;
        }

        private void HandleSearchReplaceParamsChanged()
        {
            UpdateModel();
        }

        // Public methods -----------------------------------------------------

        public SearchReplaceWindowViewModel(ISearchHost searchHost, 
            ISearchReplaceWindowAccess access, 
            IMessagingService messagingService)
        {
            this.searchHost = searchHost;
            this.access = access;
            this.messagingService = messagingService;

            search = null;
            replace = null;
            caseSensitive = false;
            wholeWordsOnly = false;
            searchMode = SearchMode.Normal;

            selectionAvailable = searchHost.SelectionAvailableCondition.GetValue();
            searchHost.SelectionAvailableCondition.ValueChanged += HandleSelectionAvailableChanged;

            searchRegexValidCondition = new Condition(true);

            FindNextCommand = new AppCommand(obj => DoFindNext(), searchHost.CanSearchCondition & searchRegexValidCondition);
            ReplaceCommand = new AppCommand(obj => DoReplace(), searchHost.CanSearchCondition & searchRegexValidCondition);
            ReplaceAllCommand = new AppCommand(obj => DoReplaceAll(), searchHost.CanSearchCondition & searchRegexValidCondition);
            CloseCommand = new AppCommand(obj => DoClose());
        }

        public void ShowSearch()
        {
            access.ChooseSearchTab();
            access.ShowAndFocus();
        }

        public void ShowReplace()
        {
            replaceAllInSelection = searchHost.SelectionAvailableCondition.GetValue();

            access.ChooseReplaceTab();
            access.ShowAndFocus();
        }

        // Public properties --------------------------------------------------

        public string Search
        {
            get => search;
            set => Set(ref search, () => Search, value, HandleSearchReplaceParamsChanged);
        }

        public string Replace
        {
            get => replace;
            set => Set(ref replace, () => Replace, value, HandleSearchReplaceParamsChanged);
        }

        public bool CaseSensitive
        {
            get => caseSensitive;
            set => Set(ref caseSensitive, () => CaseSensitive, value, HandleSearchReplaceParamsChanged);
        }

        public bool WholeWordsOnly
        {
            get => wholeWordsOnly;
            set => Set(ref wholeWordsOnly, () => WholeWordsOnly, value, HandleSearchReplaceParamsChanged);
        }

        public bool SearchBackwards
        {
            get => searchBackwards;
            set => Set(ref searchBackwards, () => SearchBackwards, value, HandleSearchReplaceParamsChanged);
        }

        public bool SelectionAvailable
        {
            get => selectionAvailable;
            set => Set(ref selectionAvailable, () => SelectionAvailable, value, HandleSearchReplaceParamsChanged);
        }

        public bool ReplaceAllInSelection
        {
            get => replaceAllInSelection;
            set => Set(ref replaceAllInSelection, () => ReplaceAllInSelection, value, HandleSearchReplaceParamsChanged);
        }

        public SearchMode SearchMode
        {
            get => searchMode;
            set => Set(ref searchMode, () => SearchMode, value, HandleSearchReplaceParamsChanged);
        }

        public ICommand FindNextCommand { get; }
        public ICommand ReplaceCommand { get; }
        public ICommand ReplaceAllCommand { get; }
        public ICommand CloseCommand { get; }
    }
}

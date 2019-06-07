using Dev.Editor.BusinessLogic.Models.Search;
using Dev.Editor.BusinessLogic.Services.Messaging;
using Dev.Editor.BusinessLogic.Types.Search;
using Dev.Editor.BusinessLogic.ViewModels.Base;
using Dev.Editor.Common.Commands;
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

        // Private methods ----------------------------------------------------

        private void DoClose() => access.Close();

        private void DoReplaceAll()
        {
            try
            {
                Regex regex = GetRegEx(search);

                var model = new ReplaceModel(regex, replace, searchBackwards);
                searchHost.ReplaceAll(model);
            }
            catch
            {
                messagingService.ShowError(Properties.Resources.Message_InvalidSearchPattern);
                return;
            }
        }

        private void DoReplace()
        {
            try
            {
                Regex regex = GetRegEx(search);

                var model = new ReplaceModel(regex, replace, searchBackwards);
                searchHost.Replace(model);
            }
            catch
            {
                messagingService.ShowError(Properties.Resources.Message_InvalidSearchPattern);
                return;
            }
        }

        private void DoFindNext()
        {
            try
            {
                Regex regex = GetRegEx(search);

                var model = new SearchModel(regex, searchBackwards);
                searchHost.FindNext(model);
            }
            catch
            {
                messagingService.ShowError(Properties.Resources.Message_InvalidSearchPattern);
                return;
            }
        }

        private Regex GetRegEx(string textToFind)
        {
            RegexOptions options = RegexOptions.None;
            if (searchBackwards)
                options |= RegexOptions.RightToLeft;
            if (!caseSensitive)
                options |= RegexOptions.IgnoreCase;

            switch (searchMode)
            {
                case SearchMode.RegularExpressions:
                    return new Regex(textToFind, options);
                case SearchMode.Extended:
                    {
                        // TODO fix
                        string pattern = textToFind;
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

        // Public methods -----------------------------------------------------

        public SearchReplaceWindowViewModel(ISearchHost searchHost, ISearchReplaceWindowAccess access, IMessagingService messagingService)
        {
            this.searchHost = searchHost;
            this.access = access;
            this.messagingService = messagingService;

            search = null;
            replace = null;
            caseSensitive = false;
            wholeWordsOnly = false;
            searchMode = SearchMode.Normal;

            FindNextCommand = new AppCommand(obj => DoFindNext(), searchHost.CanSearchCondition);
            ReplaceCommand = new AppCommand(obj => DoReplace(), searchHost.CanSearchCondition);
            ReplaceAllCommand = new AppCommand(obj => DoReplaceAll(), searchHost.CanSearchCondition);
            CloseCommand = new AppCommand(obj => DoClose());
        }

        public void ShowSearch()
        {
            access.ChooseSearchTab();
            access.ShowAndFocus();
        }

        public void ShowReplace()
        {
            access.ChooseReplaceTab();
            access.ShowAndFocus();
        }

        // Public properties --------------------------------------------------

        public string Search
        {
            get => search;
            set => Set(ref search, () => Search, value);
        }

        public string Replace
        {
            get => replace;
            set => Set(ref replace, () => Replace, value);
        }

        public bool CaseSensitive
        {
            get => caseSensitive;
            set => Set(ref caseSensitive, () => CaseSensitive, value);
        }

        public bool WholeWordsOnly
        {
            get => wholeWordsOnly;
            set => Set(ref wholeWordsOnly, () => WholeWordsOnly, value);
        }

        public bool SearchBackwards
        {
            get => searchBackwards;
            set => Set(ref searchBackwards, () => SearchBackwards, value);
        }

        public SearchMode SearchMode
        {
            get => searchMode;
            set => Set(ref searchMode, () => SearchMode, value);
        }

        public ICommand FindNextCommand { get; }
        public ICommand ReplaceCommand { get; }
        public ICommand ReplaceAllCommand { get; }
        public ICommand CloseCommand { get; }
    }
}

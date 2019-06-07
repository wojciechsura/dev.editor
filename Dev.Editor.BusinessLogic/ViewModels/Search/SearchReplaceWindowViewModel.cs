using Dev.Editor.BusinessLogic.Models.Search;
using Dev.Editor.BusinessLogic.Types.Search;
using Dev.Editor.BusinessLogic.ViewModels.Base;
using Dev.Editor.Common.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dev.Editor.BusinessLogic.ViewModels.Search
{
    public class SearchReplaceWindowViewModel : BaseViewModel
    {
        // Private fields -----------------------------------------------------

        private readonly ISearchHost searchHost;
        private readonly ISearchReplaceWindowAccess access;

        private string search;
        private string replace;
        private bool caseSensitive;
        private bool wholeWordsOnly;
        private SearchMode searchMode;

        // Private methods ----------------------------------------------------

        private SearchModel BuildSearchModel() 
            => new SearchModel
            {
                Search = Search,
                CaseSensitive = CaseSensitive,
                WholeWordsOnly = WholeWordsOnly,
                SearchMode = SearchMode
            };

        private ReplaceModel BuildReplaceModel()
            => new ReplaceModel
            {
                Search = Search,
                Replace = Replace,
                CaseSensitive = CaseSensitive,
                WholeWordsOnly = WholeWordsOnly,
                SearchMode = SearchMode
            };

        private void DoClose() => access.Close();
        private void DoReplaceAll() => searchHost.ReplaceAll(BuildReplaceModel());
        private void DoReplace() => searchHost.Replace(BuildReplaceModel());
        private void DoFindNext() => searchHost.FindNext(BuildSearchModel());

        // Public methods -----------------------------------------------------

        public SearchReplaceWindowViewModel(ISearchHost searchHost, ISearchReplaceWindowAccess access)
        {
            this.searchHost = searchHost;
            this.access = access;

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

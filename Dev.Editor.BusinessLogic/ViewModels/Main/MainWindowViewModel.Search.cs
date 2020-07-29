﻿using Dev.Editor.Resources;
using Dev.Editor.BusinessLogic.ViewModels.Document;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.ViewModels.Main
{
    public partial class MainWindowViewModel
    {
		// Private methods ----------------------------------------------------------

		private void DoSearch()
        {
            var searchViewModel = dialogService.RequestSearchReplace(this);
            searchViewModel.ShowSearch();
        }

		private void DoReplace()
        {
            var searchViewModel = dialogService.RequestSearchReplace(this);
            searchViewModel.ShowReplace();
        }

        private void DoFindNext()
        {
            InternalFindNext(documentsManager.ActiveDocument.LastSearch);
        }

        private void DoFindInFiles()
        {
            var searchViewModel = dialogService.RequestSearchReplace(this);
            searchViewModel.ShowFindInFiles();
        }
    }
}

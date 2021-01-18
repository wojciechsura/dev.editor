using Dev.Editor.BusinessLogic.ViewModels.BottomTools.SearchResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.ViewModels.DuplicatedLines
{
    public class DuplicatedContentPreviewViewModel : BaseDuplicatedLineDetailsViewModel
    {
        public DuplicatedContentPreviewViewModel(string text)
        {
            Text = text;
        }

        public string Text { get; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Dev.Editor.BusinessLogic.ViewModels.Search
{
    public class SearchResultViewModel
    {
        public SearchResultViewModel(FormattedText display, FileSearchResultViewModel parent, int line, int column) 
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            Display = display;
            Parent = parent;
            Line = line;
            Column = column;
        }

        public FormattedText Display { get; }
        public FileSearchResultViewModel Parent { get; }
        public int Line { get; }
        public int Column { get; }
    }
}

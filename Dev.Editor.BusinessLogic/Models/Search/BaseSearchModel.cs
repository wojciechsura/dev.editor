using Dev.Editor.BusinessLogic.Types.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.Search
{
    public class BaseSearchModel
    {
        public string Search { get; set; }
        public bool WholeWordsOnly { get; set; }
        public bool CaseSensitive { get; set; }
        public SearchMode SearchMode { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.ViewModels.FindInFiles
{
    public class ReplaceResultViewModel : SearchResultViewModel
    {
        private bool isChecked = true;

        public ReplaceResultViewModel(string fullPath, string before, string match, string replaceWith, string after, int line, int column, int offset, int length) 
            : base(fullPath, before, match, after, line, column, offset, length)
        {
            ReplaceWith = replaceWith;
        }

        public bool IsChecked
        {
            get => isChecked;
            set => Set(ref isChecked, () => IsChecked, value);
        }

        public string ReplaceWith { get; }
    }
}

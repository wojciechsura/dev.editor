using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Dev.Editor.BusinessLogic.ViewModels.Projects
{
    public class ProjectFolderViewModel : BaseProjectItemViewModel
    {
        private readonly IReadOnlyList<BaseProjectItemViewModel> children;

        public ProjectFolderViewModel(string path, 
            ImageSource icon, 
            IReadOnlyList<BaseProjectItemViewModel> children)
            : base(path, icon)
        {
            this.children = children;
        }

        public IReadOnlyList<BaseProjectItemViewModel> Children => children;
    }
}

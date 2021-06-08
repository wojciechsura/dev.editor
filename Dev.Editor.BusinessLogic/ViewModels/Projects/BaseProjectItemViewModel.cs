using Dev.Editor.BusinessLogic.Types.PropertyChange;
using Dev.Editor.BusinessLogic.ViewModels.Base;
using Dev.Editor.BusinessLogic.ViewModels.Tools.Project;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Dev.Editor.BusinessLogic.ViewModels.Projects
{
    public class BaseProjectItemViewModel : BaseViewModel
    {
        private string path;
        private string filename;
        private ImageSource icon;
        private bool isExpanded;
        private bool isSelected;
        private bool isIncludedByFilter;

        protected readonly IProjectHandler projectHandler;

        public BaseProjectItemViewModel(string path, ImageSource icon, IProjectHandler projectHandler)
        {
            this.path = path;
            this.icon = icon;
            this.projectHandler = projectHandler;
            filename = System.IO.Path.GetFileName(path);

            isExpanded = false;
            isSelected = false;
            isIncludedByFilter = true;
        }

        public bool IsExpanded
        {
            get => isExpanded; 
            set => Set(ref isExpanded, () => IsExpanded, value);
        }

        public bool IsSelected
        {
            get => isSelected; 
            set => Set(ref isSelected, () => IsSelected, value);
        }

        public bool IsIncludedByFilter
        {
            get => isIncludedByFilter;
            set => Set(ref isIncludedByFilter, () => IsIncludedByFilter, value);
        }

        public string Path => path;
        public string Filename => filename;
        public ImageSource Icon => icon;
        public string Display => System.IO.Path.GetFileName(Path);
    }
}

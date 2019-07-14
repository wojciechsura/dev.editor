using Dev.Editor.BusinessLogic.ViewModels.Base;
using Dev.Editor.Common.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Dev.Editor.BusinessLogic.ViewModels.Tools.Explorer
{
    public class FolderItemViewModel : BaseViewModel
    {
        // Private fields -----------------------------------------------------

        private readonly FolderItemViewModel parent;
        private readonly string display;
        private readonly string myPath;
        private readonly ImageSource icon;
        private readonly IFileItemHandler handler;
        private readonly ObservableCollection<FolderItemViewModel> children;

        private bool isInitialized;
        private bool isExpanded;
        private bool isSelected;

        // Private methods ----------------------------------------------------

        private void HandleIsExpandedChanged()
        {
            if (isExpanded && !isInitialized)
            {
                Initialize();
            }
        }

        private void Initialize()
        {
            if (!isInitialized)
            {
                children.Clear();

                try
                {
                    // Load folders
                    string currentPath = GetFullPath();
                    System.IO.Directory.EnumerateDirectories(currentPath)
                        .Select(x => System.IO.Path.GetFileName(x))
                        .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                        .Select(x => new FolderItemViewModel(this, x, x, handler.GetFolderIcon(x), handler))
                        .ForEach(item => children.Add(item));
                }
                catch (Exception)
                {
                    // Ignore - don't load items if not possible
                }

                isInitialized = true;
            }
        }

        private void HandleIsSelectedChanged()
        {
            if (isSelected)
                handler.NotifyItemSelected(this);
        }


        // Public methods -----------------------------------------------------

        public FolderItemViewModel(FolderItemViewModel parent, string display, string myPath, ImageSource icon, IFileItemHandler handler)
        {
            this.parent = parent;
            this.display = display;
            this.myPath = myPath;
            this.icon = icon;
            this.handler = handler;
            this.children = new ObservableCollection<FolderItemViewModel> { null };

            isExpanded = false;
            isInitialized = false;
        }

        public string GetFullPath()
        {
            string path;

            if (parent != null)
                path = System.IO.Path.Combine(parent.GetFullPath(), myPath);
            else
                path = myPath;

            if (!path.EndsWith(@"\"))
                path += @"\";

            return path;
        }

        // Public properties --------------------------------------------------

        public string Path => myPath;

        public string Display => display;

        public ImageSource Icon => icon;

        public bool IsExpanded
        {
            get => isExpanded;
            set
            {
                Set(ref isExpanded, () => IsExpanded, value, HandleIsExpandedChanged);
            }
        }

        public bool IsSelected
        {
            get => isSelected;
            set
            {
                Set(ref isSelected, () => IsSelected, value, HandleIsSelectedChanged);
            }
        }

        public FolderItemViewModel Parent => parent;

        public ObservableCollection<FolderItemViewModel> Children => children;
    }
}

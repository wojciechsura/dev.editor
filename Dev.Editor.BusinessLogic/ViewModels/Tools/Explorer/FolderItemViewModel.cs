using Dev.Editor.BusinessLogic.ViewModels.Base;
using Dev.Editor.Common.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;

namespace Dev.Editor.BusinessLogic.ViewModels.Tools.Explorer
{
    public class FolderItemViewModel : BaseViewModel
    {
        // Private fields -----------------------------------------------------

        private readonly ObservableCollection<FolderItemViewModel> children;
        private readonly string display;
        private readonly IFileItemHandler handler;
        private readonly ImageSource icon;
        private readonly FolderItemViewModel parent;
        private readonly string path;
        private bool isExpanded;
        private bool isInitialized;
        private bool isSelected;

        // Private methods ----------------------------------------------------

        private List<FolderItemViewModel> GetSubFolders()
        {
            string currentPath = GetFullPath();

            var contents = System.IO.Directory.EnumerateDirectories(currentPath)
                .Select(x => System.IO.Path.GetFileName(x))
                .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                .Select(x => new FolderItemViewModel(this, x, x, handler.GetFolderIcon(x), handler))
                .ToList();
            return contents;
        }

        private void HandleIsExpandedChanged()
        {
            if (isExpanded && !isInitialized)
            {
                Initialize();
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
            this.path = myPath;
            this.icon = icon;
            this.handler = handler;
            this.children = new ObservableCollection<FolderItemViewModel> { null };

            isExpanded = false;
            isInitialized = false;
        }

        public string GetFullPath()
        {
            string result;

            if (parent != null)
                result = System.IO.Path.Combine(parent.GetFullPath(), path);
            else
                result = path;

            if (!result.EndsWith(@"\"))
                result += @"\";

            return result;
        }

        public void Initialize()
        {
            if (!isInitialized)
            {
                children.Clear();

                try
                {
                    // Load folders
                    List<FolderItemViewModel> subFolders = GetSubFolders();

                    subFolders.ForEach(item => children.Add(item));
                }
                catch (Exception)
                {
                    // Ignore - don't load items if not possible
                }

                isInitialized = true;
            }
        }

        public void RefreshRecursive()
        {
            // Refresh only opened folders
            if (!isInitialized)
                return;

            try
            {
                var updated = GetSubFolders();

                FolderListHelper.UpdateFolderList(children, updated);

                foreach (var child in children)
                    if (child.IsInitialized)
                        child.RefreshRecursive();
            }
            catch
            {
                // Ignore - don't load items if not possible
            }
        }

        // Public properties --------------------------------------------------

        public ObservableCollection<FolderItemViewModel> Children => children;

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

        public bool IsInitialized => isInitialized;

        public bool IsSelected
        {
            get => isSelected;
            set
            {
                Set(ref isSelected, () => IsSelected, value, HandleIsSelectedChanged);
            }
        }

        public FolderItemViewModel Parent => parent;

        public string Path => path;
    }
}

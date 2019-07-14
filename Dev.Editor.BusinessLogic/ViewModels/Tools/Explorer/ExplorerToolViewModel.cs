using Dev.Editor.BusinessLogic.Services.FileIcons;
using Dev.Editor.BusinessLogic.Services.ImageResources;
using Dev.Editor.BusinessLogic.Types.Tools.Explorer;
using Dev.Editor.BusinessLogic.ViewModels.Tools.Base;
using Dev.Editor.Common.Tools;
using Dev.Editor.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Dev.Editor.BusinessLogic.ViewModels.Tools.Explorer
{
    public class ExplorerToolViewModel : BaseToolViewModel, IFileItemHandler
    {
        private readonly IFileIconProvider fileIconProvider;
        private readonly IImageResources imageResources;
        private readonly ImageSource icon;

        private readonly ObservableCollection<FolderItemViewModel> folders;
        private FolderItemViewModel selectedFolder;
        private readonly ObservableCollection<FileItemViewModel> files;

        private void InitializeFolders()
        {
            folders.Clear();
            System.IO.Directory.GetLogicalDrives()
                .Select(x => $"{x[0]}:")
                .OrderBy(x => x.ToLower())
                .Select(x => new FolderItemViewModel(null, x, x, fileIconProvider.GetImageForFolder(x), this))
                .ForEach(x => folders.Add(x));            
        }

        private void HandleSelectedFolderChanged()
        {
            files.Clear();

            try
            {
                if (selectedFolder != null)
                {
                    if (selectedFolder.Parent != null)
                    {
                        files.Add(new FileItemViewModel(selectedFolder.Parent.GetFullPath(), "..", imageResources.GetIconByName("Up16.png"), FileItemType.ParentFolder));
                    }

                    System.IO.Directory.EnumerateDirectories(selectedFolder.GetFullPath())
                        .Select(x => new { Path = x, Display = System.IO.Path.GetFileName(x) })
                        .OrderBy(x => x.Display, StringComparer.OrdinalIgnoreCase)
                        .Select(x => new FileItemViewModel(x.Path, x.Display, fileIconProvider.GetImageForFolder(x.Display), FileItemType.Folder))
                        .ForEach(x => files.Add(x));

                    System.IO.Directory.EnumerateFiles(selectedFolder.GetFullPath())
                        .Select(x => new { Path = x, Display = System.IO.Path.GetFileName(x) })
                        .OrderBy(x => x.Display, StringComparer.OrdinalIgnoreCase)
                        .Select(x => new FileItemViewModel(x.Path, x.Display, fileIconProvider.GetImageForFile(x.Display), FileItemType.File))
                        .ForEach(x => files.Add(x));
                }
            }
            catch (Exception)
            {
                // Don't display files from this folder if it is not possible
            }
        }

        public void NotifyItemSelected(FolderItemViewModel folderItemViewModel)
        {
            SelectedFolder = folderItemViewModel;
        }

        public ExplorerToolViewModel(IFileIconProvider fileIconProvider, IImageResources imageResources)
        {
            this.fileIconProvider = fileIconProvider;
            this.imageResources = imageResources;

            this.icon = imageResources.GetIconByName("Explorer16.png");

            folders = new ObservableCollection<FolderItemViewModel>();
            files = new ObservableCollection<FileItemViewModel>();
            InitializeFolders();
        }

        public ImageSource GetFolderIcon(string name)
        {
            return fileIconProvider.GetImageForFolder(name);
        }

        public override string Title => Strings.Tool_Explorer_Title;

        public override ImageSource Icon => icon;

        public ObservableCollection<FolderItemViewModel> Folders => folders;

        public FolderItemViewModel SelectedFolder
        {
            get => selectedFolder;
            set
            {
                Set(ref selectedFolder, () => SelectedFolder, value, HandleSelectedFolderChanged);
            }
        }

        public ObservableCollection<FileItemViewModel> Files => files;
    }
}

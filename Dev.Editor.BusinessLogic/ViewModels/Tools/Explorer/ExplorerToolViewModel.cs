using Dev.Editor.BusinessLogic.Services.FileIcons;
using Dev.Editor.BusinessLogic.Services.ImageResources;
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

        private void InitializeFolders()
        {
            folders.Clear();
            System.IO.Directory.GetLogicalDrives()
                .OrderBy(x => x.ToLower())
                .Select(x => new FolderItemViewModel(null, x, x, fileIconProvider.GetImageForFolder(x), this))
                .ForEach(x => folders.Add(x));            
        }

        public ExplorerToolViewModel(IFileIconProvider fileIconProvider, IImageResources imageResources)
        {
            this.fileIconProvider = fileIconProvider;
            this.imageResources = imageResources;

            this.icon = imageResources.GetIconByName("Explorer16.png");

            folders = new ObservableCollection<FolderItemViewModel>();
            InitializeFolders();
        }

        public ImageSource GetFolderIcon(string name)
        {
            return fileIconProvider.GetImageForFolder(name);
        }

        public override string Title => Strings.Tool_Explorer_Title;

        public override ImageSource Icon => icon;

        public ObservableCollection<FolderItemViewModel> Folders => folders;
    }
}

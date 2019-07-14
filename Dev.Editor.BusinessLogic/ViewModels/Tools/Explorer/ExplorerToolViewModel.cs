﻿using Dev.Editor.BusinessLogic.Services.FileIcons;
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
        private IExplorerToolAccess access;

        private readonly ObservableCollection<FolderItemViewModel> folders;
        private FolderItemViewModel selectedFolder;
        private readonly ObservableCollection<FileItemViewModel> files;
        private FileItemViewModel selectedFile;

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
                        files.Add(new FileItemViewModel(selectedFolder, "..", "..", imageResources.GetIconByName("Up16.png"), FileItemType.ParentFolder));
                    }

                    System.IO.Directory.EnumerateDirectories(selectedFolder.GetFullPath())
                        .Select(x => System.IO.Path.GetFileName(x))
                        .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                        .Select(x => new FileItemViewModel(selectedFolder, x, $"[ {x} ]", fileIconProvider.GetImageForFolder(x), FileItemType.Folder))
                        .ForEach(x => files.Add(x));

                    System.IO.Directory.EnumerateFiles(selectedFolder.GetFullPath())
                        .Select(x => System.IO.Path.GetFileName(x))
                        .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                        .Select(x => new FileItemViewModel(selectedFolder, x, x, fileIconProvider.GetImageForFile(x), FileItemType.File))
                        .ForEach(x => files.Add(x));
                }
            }
            catch (Exception)
            {
                // Don't display files from this folder if it is not possible
            }

            SelectedFile = files.FirstOrDefault();
        }

        private void OpenSubfolder(string name)
        {
            if (selectedFolder != null)
            {
                selectedFolder.IsExpanded = true;

                var subfolder = selectedFolder.Children
                    .FirstOrDefault(x => x.Path.Equals(name, StringComparison.OrdinalIgnoreCase));

                if (subfolder != null)
                {
                    subfolder.IsSelected = true;
                }

                SelectedFile = files.FirstOrDefault();
            }
        }

        private void OpenParentFolder()
        {
            if (selectedFolder != null && selectedFolder.Parent != null)
            {
                string subfolderName = selectedFolder.Path;

                selectedFolder.Parent.IsSelected = true;

                var file = files.FirstOrDefault(f => f.Path.Equals(subfolderName, StringComparison.OrdinalIgnoreCase));
                if (file != null)
                    SelectedFile = file;
                else
                    SelectedFile = files.FirstOrDefault();
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

        public void FileItemChosen()
        {
            if (selectedFile != null)
            {
                switch (selectedFile.ItemType)
                {
                    case FileItemType.File:
                        {
                            break;
                        }
                    case FileItemType.Folder:
                        {
                            OpenSubfolder(System.IO.Path.GetFileName(selectedFile.Path));
                            break;
                        }
                    case FileItemType.ParentFolder:
                        {
                            OpenParentFolder();
                            break;
                        }
                }
            }
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

        public FileItemViewModel SelectedFile
        {
            get => selectedFile;
            set
            {
                Set(ref selectedFile, () => SelectedFile, value);
            }
        }

        public IExplorerToolAccess Access
        {
            get => access;
            set
            {
                access = value;
            }
        }
    }
}

using Dev.Editor.BusinessLogic.Models.Events;
using Dev.Editor.BusinessLogic.Services.Config;
using Dev.Editor.BusinessLogic.Services.EventBus;
using Dev.Editor.BusinessLogic.Services.FileIcons;
using Dev.Editor.BusinessLogic.Services.ImageResources;
using Dev.Editor.BusinessLogic.Services.Platform;
using Dev.Editor.BusinessLogic.Types.Tools.Explorer;
using Dev.Editor.BusinessLogic.ViewModels.Tools.Base;
using Spooksoft.VisualStateManager.Commands;
using Spooksoft.VisualStateManager.Conditions;
using Dev.Editor.Common.Tools;
using Dev.Editor.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace Dev.Editor.BusinessLogic.ViewModels.Tools.Explorer
{
    public class ExplorerToolViewModel : BaseToolViewModel, IFileItemHandler, IEventListener<ApplicationActivatedEvent>
    {
        // Private fields -----------------------------------------------------

        private readonly IConfigurationService configurationService;
        private readonly IEventBus eventBus;

        private readonly IExplorerHandler explorerHandler;
        private readonly IFileIconProvider fileIconProvider;
        private readonly ObservableCollection<FileItemViewModel> files;
        private readonly ObservableCollection<FolderItemViewModel> folders;
        private readonly ImageSource icon;
        private readonly IImageResources imageResources;
        private readonly IPlatformService platformService;

        private readonly SimpleCondition fileSelectedCondition;
        private readonly SimpleCondition folderSelectedCondition;

        private IExplorerToolAccess access;
        private double folderTreeHeight;
        private FileItemViewModel selectedFile;
        private FolderItemViewModel selectedFolder;

        // Private methods ----------------------------------------------------

        private void DoOpenFolderInExplorer()
        {
            var path = selectedFolder.GetFullPath();
            platformService.ShowInExplorer(path);
        }

        private void DoSelectFileInExplorer()
        {
            var path = selectedFile.GetFullPath();
            platformService.SelectInExplorer(path);
        }

        private void DoSetLocationOfCurrentDocument()
        {
            string path = explorerHandler.GetCurrentDocumentPath();

            ShowCurrentPath(path);
        }

        private void HandleFolderTreeHeightChanged()
        {
            configurationService.Configuration.Tools.Explorer.FolderTreeHeight.Value = folderTreeHeight;
        }

        private void HandleSelectedFileChanged()
        {
            fileSelectedCondition.Value = selectedFile != null;
        }

        private void HandleSelectedFolderChanged()
        {
            folderSelectedCondition.Value = selectedFolder != null;
            configurationService.Configuration.Tools.Explorer.LastFolder.Value = selectedFolder?.GetFullPath();

            UpdateFolderContents();
        }

        private void InitializeFolders()
        {
            folders.Clear();
            System.IO.Directory.GetLogicalDrives()
                .Select(x => $"{x[0]}:")
                .OrderBy(x => x.ToLower())
                .Select(x => new FolderItemViewModel(null, x, x, fileIconProvider.GetImageForFolder(x), this))
                .ForEach(x => folders.Add(x));
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

                access.FixListboxFocus();
            }
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
                access.FixListboxFocus();
            }
        }

        private void RefreshDrives()
        {
            var drives = System.IO.Directory.GetLogicalDrives()
                            .Select(x => $"{x[0]}:")
                            .OrderBy(x => x.ToLower())
                            .Select(x => new FolderItemViewModel(null, x, x, fileIconProvider.GetImageForFolder(x), this))
                            .ToList();

            FolderListHelper.UpdateFolderList(folders, drives);
        }

        private void RefreshFolders()
        {
            // Updating drives
            RefreshDrives();

            // Refresh opened folders
            foreach (var folder in folders)
            {
                folder.RefreshRecursive();
            }

            // Refresh current folder contents
            UpdateFolderContents();
        }

        private void SetCurrentPath(string path)
        {
            var pathFragments = path.Split('\\');

            FolderItemViewModel current = null;

            for (int i = 0; i < pathFragments.Length - 1; i++)
            {
                var folder = (current?.Children ?? folders).FirstOrDefault(f => f.Path.ToLower().Equals(pathFragments[i].ToLower()));

                if (folder == null)
                    return;

                folder.IsExpanded = true;
                current = folder;
            }

            if (SelectedFolder != null)
                SelectedFolder.IsSelected = false;
            SelectedFolder = null;

            current.IsSelected = true;

            var file = files.FirstOrDefault(f => f.Path.ToLower().Equals(pathFragments.Last().ToLower()));
            if (file != null)
                SelectedFile = file;

        }

        private void ShowCurrentPath(string path)
        {
            SetCurrentPath(path);

            access.ScrollToSelectedFolder();
            access.ScrollToSelectedFile();
        }

        private void UpdateFolderContents()
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
                        .Select(x => new FileItemViewModel(selectedFolder, x, $"[{x}]", fileIconProvider.GetImageForFolder(x), FileItemType.Folder))
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

        // Public metods ------------------------------------------------------

        public ExplorerToolViewModel(IFileIconProvider fileIconProvider, 
            IImageResources imageResources, 
            IConfigurationService configurationService,
            IExplorerHandler handler,
            IEventBus eventBus,
            IPlatformService platformService)
            : base(handler)
        {
            this.fileIconProvider = fileIconProvider;
            this.imageResources = imageResources;
            this.configurationService = configurationService;
            this.explorerHandler = handler;
            this.eventBus = eventBus;
            this.platformService = platformService;

            eventBus.Register((IEventListener<ApplicationActivatedEvent>)this);

            this.icon = imageResources.GetIconByName("Explorer16.png");

            folders = new ObservableCollection<FolderItemViewModel>();
            files = new ObservableCollection<FileItemViewModel>();
            InitializeFolders();

            folderTreeHeight = configurationService.Configuration.Tools.Explorer.FolderTreeHeight.Value;

            folderSelectedCondition = new SimpleCondition(selectedFolder != null);
            fileSelectedCondition = new SimpleCondition(selectedFile != null);

            SetLocationOfCurrentDocumentCommand = new AppCommand(obj => DoSetLocationOfCurrentDocument(), handler.CurrentDocumentHasPathCondition);
            OpenFolderInExplorerCommand = new AppCommand(obj => DoOpenFolderInExplorer(), folderSelectedCondition);
            SelectFileInExplorerCommand = new AppCommand(obj => DoSelectFileInExplorer(), fileSelectedCondition);
            OpenTextCommand = new AppCommand(obj => DoOpenText(), fileSelectedCondition);
            OpenHexCommand = new AppCommand(obj => DoOpenHex(), fileSelectedCondition);
            ExecuteCommand = new AppCommand(obj => DoExecute(), fileSelectedCondition);

            if (configurationService.Configuration.Tools.Explorer.LastFolder.Value != null)
            {
                SetCurrentPath(configurationService.Configuration.Tools.Explorer.LastFolder.Value);
            }
        }

        private void DoOpenHex()
        {
            explorerHandler.OpenHexFile(selectedFile.GetFullPath());
        }

        private void DoOpenText()
        {
            explorerHandler.OpenTextFile(selectedFile.GetFullPath());
        }

        private void DoExecute()
        {
            explorerHandler.Execute(selectedFile.GetFullPath());
        }

        public void FileItemChosen()
        {
            if (selectedFile != null)
            {
                switch (selectedFile.ItemType)
                {
                    case FileItemType.File:
                        {
                            explorerHandler.OpenTextFile(selectedFile.GetFullPath());
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

        public ImageSource GetFolderIcon(string name)
        {
            return fileIconProvider.GetImageForFolder(name);
        }

        public void NotifyItemSelected(FolderItemViewModel folderItemViewModel)
        {
            SelectedFolder = folderItemViewModel;
        }

        void IEventListener<ApplicationActivatedEvent>.Receive(ApplicationActivatedEvent @event)
        {
            RefreshFolders();
        }

        public void NotifyWindowLoaded()
        {
            // When view is attached and ready, scroll to the selected file and folder.

            access.ScrollToSelectedFolder();
            access.ScrollToSelectedFile();
        }

        public void Refresh()
        {
            RefreshFolders();
        }

        // Public properties --------------------------------------------------

        public IExplorerToolAccess Access
        {
            get => access;
            set
            {
                access = value;
            }
        }

        public ObservableCollection<FileItemViewModel> Files => files;

        public ObservableCollection<FolderItemViewModel> Folders => folders;

        public double FolderTreeHeight
        {
            get => folderTreeHeight;
            set
            {
                Set(ref folderTreeHeight, () => FolderTreeHeight, value, HandleFolderTreeHeightChanged);
            }
        }

        public override ImageSource Icon => icon;

        public ICommand OpenFolderInExplorerCommand { get; }

        public FileItemViewModel SelectedFile
        {
            get => selectedFile;
            set
            {
                Set(ref selectedFile, () => SelectedFile, value, HandleSelectedFileChanged);
            }
        }
        
        public FolderItemViewModel SelectedFolder
        {
            get => selectedFolder;
            set
            {
                Set(ref selectedFolder, () => SelectedFolder, value, HandleSelectedFolderChanged);
            }
        }

        public ICommand SelectFileInExplorerCommand { get; }

        public ICommand SetLocationOfCurrentDocumentCommand { get; }

        public ICommand OpenTextCommand { get; }

        public ICommand OpenHexCommand { get; }

        public ICommand ExecuteCommand { get; }

        public override string Title => Strings.Tool_Explorer_Title;

        public override string Uid => ExplorerUid;
    }
}

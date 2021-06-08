using Dev.Editor.BusinessLogic.Models.Events;
using Dev.Editor.BusinessLogic.Services.EventBus;
using Dev.Editor.BusinessLogic.Services.FileIcons;
using Dev.Editor.BusinessLogic.Services.ImageResources;
using Dev.Editor.BusinessLogic.ViewModels.Projects;
using Dev.Editor.BusinessLogic.ViewModels.Tools.Base;
using Dev.Editor.Resources;
using Spooksoft.VisualStateManager.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace Dev.Editor.BusinessLogic.ViewModels.Tools.Project
{
    public class ProjectToolViewModel : BaseToolViewModel, IEventListener<ApplicationActivatedEvent>
    {
        // Private types ------------------------------------------------------

        private class LoaderModel
        {
            public LoaderModel(string location)
            {
                Location = location;
            }

            public string Location { get; }
        }

        private class LoaderResultModel
        {
            public LoaderResultModel(List<BaseProjectItemViewModel> items, string location)
            {
                Items = items;
                Location = location;
            }

            public List<BaseProjectItemViewModel> Items { get; }
            public string Location { get; }
        }

        private class ProjectLoader : BackgroundWorker
        {
            private readonly IFileIconProvider fileIconProvider;
            private readonly IProjectHandler projectHandler;

            private List<BaseProjectItemViewModel> LoadDirectory(string location)
            {
                var result = new List<BaseProjectItemViewModel>();

                foreach (var folder in Directory.EnumerateDirectories(location).OrderBy(d => System.IO.Path.GetFileName(d)))
                {
                    if (CancellationPending)
                        return null;

                    var childItems = LoadDirectory(folder);
                    var item = new ProjectFolderViewModel(folder, fileIconProvider.GetImageForFolder(System.IO.Path.GetFileName(folder)), childItems, projectHandler);
                    result.Add(item);
                }

                foreach (var file in Directory.EnumerateFiles(location).OrderBy(f => System.IO.Path.GetFileName(f)))
                {
                    if (CancellationPending)
                        return null;

                    var item = new ProjectFileViewModel(file, fileIconProvider.GetImageForFile(System.IO.Path.GetFileName(file)), projectHandler);
                    result.Add(item);
                }

                return result;
            }

            protected override void OnDoWork(DoWorkEventArgs e)
            {
                var model = e.Argument as LoaderModel;

                List<BaseProjectItemViewModel> rootItems = LoadDirectory(model.Location);

                if (CancellationPending)
                    e.Result = null;
                else
                    e.Result = rootItems;
            }

            public ProjectLoader(IFileIconProvider fileIconProvider, IProjectHandler projectHandler)
            {
                WorkerSupportsCancellation = true;
                
                this.fileIconProvider = fileIconProvider;
                this.projectHandler = projectHandler;
            }
        }

        // Private fields -----------------------------------------------------

        private readonly IProjectHandler projectHandler;
        private readonly IEventBus eventBus;
        private readonly IImageResources imageResources;
        private readonly ImageSource icon;
        private readonly IFileIconProvider fileIconProvider;

        private bool opened;
        private string path;
        private IReadOnlyList<BaseProjectItemViewModel> items;

        private BackgroundWorker projectLoader;
        private string projectFilter;

        // Private methods ----------------------------------------------------

        private void ProjectLoader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            items = e.Result as List<BaseProjectItemViewModel>;
            OnPropertyChanged(() => Items);

            opened = true;
        }

        private void DoOpenFolderAsProject()
        {
            projectHandler.OpenFolderAsProject();
        }

        private void FindProjectItemsRecursive(IReadOnlyList<BaseProjectItemViewModel> items, string navigationText, List<ProjectFileViewModel> result)
        {
            foreach (var item in items)
            {
                if (item is ProjectFolderViewModel folder)
                {
                    FindProjectItemsRecursive(folder.Children, navigationText, result);
                }
                else if (item is ProjectFileViewModel file)
                {
                    if (file.Filename.ToLower().Contains(navigationText.ToLower()))
                        result.Add(file);
                    else
                    {
                        var filenameCaps = new string(file.Filename.Where(c => char.IsUpper(c)).ToArray());
                        if (filenameCaps.Contains(navigationText.ToUpper()))
                            result.Add(file);
                    }
                }
                else
                    throw new InvalidOperationException("Unsupported project item!");
            }
        }

        private void ClearFilterRecursive(IReadOnlyList<BaseProjectItemViewModel> items)
        {
            foreach (var item in items)
            {
                if (item is ProjectFileViewModel file)
                {
                    file.IsIncludedByFilter = true;
                }
                else if (item is ProjectFolderViewModel folder)
                {
                    folder.IsIncludedByFilter = true;
                    ClearFilterRecursive(folder.Children);
                }
                else
                    throw new InvalidOperationException("Unsupported project item!");
            }
        }

        private bool ApplyFilterRecursive(IReadOnlyList<BaseProjectItemViewModel> items, string filter)
        {

            bool anyChildIncluded = false;

            foreach (var item in items)
            {
                if (item is ProjectFileViewModel file)
                {
                    file.IsIncludedByFilter = System.IO.Path.GetFileName(file.Path).ToLower().Contains(filter.ToLower());
                }
                else if (item is ProjectFolderViewModel folder)
                {
                    // Folder is shown if any of its subfolders contains item, which matches filter
                    folder.IsIncludedByFilter = ApplyFilterRecursive(folder.Children, filter);
                }
                else
                    throw new InvalidOperationException("Unsupported project item!");

                anyChildIncluded |= item.IsIncludedByFilter;
            }

            return anyChildIncluded;

        }

        private void HandleFilterChanged()
        {
            if (opened)
            {
                if (string.IsNullOrEmpty(projectFilter))
                    ClearFilterRecursive(items);
                else
                    ApplyFilterRecursive(items, projectFilter);

                OnPropertyChanged(() => Items);
            }
        }

        // Public methods -----------------------------------------------------

        public ProjectToolViewModel(IToolHandler handler, 
            IProjectHandler projectHandler,
            IEventBus eventBus, 
            IImageResources imageResources,
            IFileIconProvider fileIconProvider)
            : base(handler)
        {
            this.projectHandler = projectHandler;
            this.eventBus = eventBus;
            this.imageResources = imageResources;
            this.fileIconProvider = fileIconProvider;

            items = null;
            opened = false;
            path = null;

            eventBus.Register((IEventListener<ApplicationActivatedEvent>)this);

            this.icon = imageResources.GetIconByName("Open16.png");

            OpenFolderAsProjectCommand = new AppCommand(obj => DoOpenFolderAsProject());
        }

        public List<ProjectFileViewModel> FindMatching(string navigationText)
        {
            var result = new List<ProjectFileViewModel>();

            if (opened)
            {
                FindProjectItemsRecursive(items, navigationText, result);
            }

            return result;
        }

        public void Receive(ApplicationActivatedEvent @event)
        {
            #warning TODO
        }

        public void OpenProject(string location)
        {
            CloseProject();

            if (projectLoader != null)
            {
                if (projectLoader.IsBusy)
                    projectLoader.CancelAsync();
            }

            projectLoader = new ProjectLoader(fileIconProvider, projectHandler);
            projectLoader.RunWorkerCompleted += ProjectLoader_RunWorkerCompleted;

            var model = new LoaderModel(location);
            projectLoader.RunWorkerAsync(model);
        }

        public void CloseProject()
        {
            Path = null;
            Opened = false;
            
            items = null;
            OnPropertyChanged(() => Items);
        }

        // Public properties --------------------------------------------------

        public override string Title => Strings.Tool_Project_Title;

        public override ImageSource Icon => icon;

        public override string Uid => ProjectUid;

        public bool Opened
        {
            get => opened;
            set => Set(ref opened, () => Opened, value);
        }

        public string Path
        {
            get => path;
            set => Set(ref path, () => Path, value);
        }

        public IEnumerable<BaseProjectItemViewModel> Items => items.Where(i => i.IsIncludedByFilter);

        public ICommand OpenFolderAsProjectCommand { get; }

        public string ProjectFilter
        {
            get => projectFilter;
            set => Set(ref projectFilter, () => ProjectFilter, value, HandleFilterChanged);
        }
    }
}

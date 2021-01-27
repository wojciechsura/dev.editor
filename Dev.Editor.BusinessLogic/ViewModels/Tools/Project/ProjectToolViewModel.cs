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

        // Private methods ----------------------------------------------------

        private void ProjectLoader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Items = e.Result as List<BaseProjectItemViewModel>;
        }

        private void DoOpenFolderAsProject()
        {
            projectHandler.OpenFolderAsProject();
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

        public void Receive(ApplicationActivatedEvent @event)
        {
            // TODO
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
            Items = null;
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

        public IReadOnlyList<BaseProjectItemViewModel> Items
        {
            get => items;
            set => Set(ref items, () => Items, value);
        }

        public ICommand OpenFolderAsProjectCommand { get; }
    }
}

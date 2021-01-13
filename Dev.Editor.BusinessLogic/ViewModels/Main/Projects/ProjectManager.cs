using Dev.Editor.BusinessLogic.Types.PropertyChange;
using Dev.Editor.BusinessLogic.ViewModels.Projects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.ViewModels.Main.Projects
{
    public class ProjectManager : BasePropertyChangeNotifier
    {
        // Private fields -----------------------------------------------------

        private bool opened;
        private string path;
        private IReadOnlyList<BaseProjectItemViewModel> children;

        private BackgroundWorker projectLoader;

        // Public methods -----------------------------------------------------

        public ProjectManager()
        {
            opened = false;
            path = null;
        }

        public void CloseProject()
        {
            Path = null;
            Opened = false;
        }

        public void OpenProject(string location)
        {
            CloseProject();

            if (projectLoader != null)
            {
                if (projectLoader.IsBusy)
                    projectLoader.CancelAsync();
            }

            projectLoader = new BackgroundWorker
            {
                WorkerSupportsCancellation = true
            };
            projectLoader.DoWork += ProjectLoader_DoWork;
            projectLoader.RunWorkerCompleted += ProjectLoader_RunWorkerCompleted;
        }

        private void ProjectLoader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // TODO
            throw new NotImplementedException();
        }

        private void ProjectLoader_DoWork(object sender, DoWorkEventArgs e)
        {
            // TODO
            throw new NotImplementedException();
        }

        // Public properties --------------------------------------------------

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

        public IReadOnlyList<BaseProjectItemViewModel> Children
        {
            get => children;
            set => Set(ref children, () => Children, value);
        }
    }
}

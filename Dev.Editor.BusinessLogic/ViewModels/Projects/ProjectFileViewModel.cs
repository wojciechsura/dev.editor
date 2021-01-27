using Dev.Editor.BusinessLogic.ViewModels.Tools.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Dev.Editor.BusinessLogic.ViewModels.Projects
{
    public class ProjectFileViewModel : BaseProjectItemViewModel
    {
        public ProjectFileViewModel(string path, ImageSource icon, IProjectHandler projectHandler) 
            : base(path, icon, projectHandler)
        {

        }

        public void NotifyDoubleClicked()
        {
            projectHandler.TryOpenFile(Path);
        }
    }
}

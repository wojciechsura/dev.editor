using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.ViewModels.Tools.Project
{
    public interface IProjectHandler
    {
        void OpenFolderAsProject();
        void TryOpenFile(string path);
    }
}

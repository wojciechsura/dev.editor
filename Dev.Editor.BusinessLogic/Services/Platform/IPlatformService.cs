using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Services.Platform
{
    public interface IPlatformService
    {
        void ShowInExplorer(string path);
        void SelectInExplorer(string path);
        void Execute(string path);
    }
}

using Dev.Editor.BusinessLogic.Services.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.Services.Platform
{
    public class PlatformService : IPlatformService
    {
        public void SelectInExplorer(string path)
        {
            System.Diagnostics.Process.Start("explorer.exe", $"/select, \"{path}\"");
        }

        public void ShowInExplorer(string path)
        {
            System.Diagnostics.Process.Start("explorer.exe", $"/open, \"{path}\"");
        }

        public void Execute(string path)
        {
            System.Diagnostics.Process.Start(path);
        }
    }
}

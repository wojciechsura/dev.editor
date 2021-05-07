using Dev.Editor.BusinessLogic.Services.Platform;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.Services.Platform
{
    public class PlatformService : IPlatformService
    {
        public void SelectInExplorer(string path)
        {
            Process.Start("explorer.exe", $"/select, \"{path}\"");
        }

        public void ShowInExplorer(string path)
        {
            Process.Start("explorer.exe", $"/open, \"{path}\"");
        }

        public void Execute(string path)
        {
            var startInfo = new ProcessStartInfo();
            startInfo.FileName = path;
            startInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(path);
            Process.Start(startInfo);
        }
    }
}

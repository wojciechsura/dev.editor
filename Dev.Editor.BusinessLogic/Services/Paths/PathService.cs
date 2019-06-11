using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Dev.Editor.BusinessLogic.Services.Paths
{
    class PathService : IPathService
    {
        private const string PUBLISHER = "Spooksoft";
        private const string APPNAME = "Dev.Editor";

        private const string CONFIG_FILENAME = "Config.xml";

        private string appDataPath;
        private string configPath;

        public PathService()
        {
            appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), PUBLISHER, APPNAME);
            Directory.CreateDirectory(appDataPath);

            configPath = Path.Combine(appDataPath, CONFIG_FILENAME);
        }

        public string ConfigPath => configPath;
    }
}

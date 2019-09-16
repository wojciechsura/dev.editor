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
        private const string STORED_FILES_FOLDER = "StoredFiles";
        private const string BIN_DEFINITIONS_FOLDER = "BinDefinitions";

        private string appDataPath;
        private string configPath;
        private string storedFilesPath;
        private string binDefinitionsPath;

        public PathService()
        {
            appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), PUBLISHER, APPNAME);
            Directory.CreateDirectory(appDataPath);

            storedFilesPath = Path.Combine(appDataPath, STORED_FILES_FOLDER);
            Directory.CreateDirectory(storedFilesPath);

            binDefinitionsPath = Path.Combine(appDataPath, BIN_DEFINITIONS_FOLDER);
            Directory.CreateDirectory(binDefinitionsPath);

            configPath = Path.Combine(appDataPath, CONFIG_FILENAME);
        }

        public string ConfigPath => configPath;

        public string StoredFilesPath => storedFilesPath;

        public string BinDefinitionsPath => binDefinitionsPath;
    }
}

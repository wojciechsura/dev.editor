using Dev.Editor.BusinessLogic.Models.Configuration;
using Dev.Editor.BusinessLogic.Services.Paths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Services.Config
{
    class ConfigrationService : IConfigurationService
    {
        private readonly IPathService pathService;
        private readonly ConfigurationModel config;

        public ConfigrationService(IPathService pathService)
        {
            this.pathService = pathService;
            this.config = new ConfigurationModel();

            var configPath = pathService.ConfigPath;
            try
            {
                config.Load(configPath);
            }
            catch
            {
                config.SetDefaults();
            }
        }

        public bool Save()
        {
            try
            {
                config.Save(pathService.ConfigPath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public ConfigurationModel Configuration => config;
    }
}

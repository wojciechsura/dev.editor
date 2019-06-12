using Dev.Editor.BusinessLogic.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Services.Config
{
    public interface IConfigurationService
    {
        ConfigurationModel Configuration { get; }

        bool Save();
    }
}

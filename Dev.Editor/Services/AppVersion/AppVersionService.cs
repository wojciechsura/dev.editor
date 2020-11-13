using Dev.Editor.BusinessLogic.Services.AppVersion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.Services.AppVersion
{
    class AppVersionService : IAppVersionService
    {
        public Version GetAppVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version;            
        }
    }
}

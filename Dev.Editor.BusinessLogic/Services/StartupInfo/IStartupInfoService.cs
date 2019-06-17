using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Services.StartupInfo
{
    public interface IStartupInfoService
    {
        string[] Parameters { get; set; }
    }
}

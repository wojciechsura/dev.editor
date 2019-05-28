using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace Dev.Editor.Dependencies
{
    public static class Configuration
    {
        private static bool configured = false;

        public static void Configure(IUnityContainer container)
        {
            if (configured)
                return;

            Dev.Editor.BusinessLogic.Dependencies.Configuration.Configure(container);

            configured = true;
        }
    }
}

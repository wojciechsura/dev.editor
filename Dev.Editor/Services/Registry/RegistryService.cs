using Dev.Editor.BusinessLogic.Services.Registry;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.Services.Registry
{
    class RegistryService : IRegistryService
    {
        private string GetRegPath(string fileType, string shellKeyName)
        {
            return $@"{fileType}\shell\{shellKeyName}";
        }

        public void RegisterFileContextMenuEntry(string fileType,
                string shellKeyName, 
                string menuText, 
                string menuCommand,
                string iconPath = null)
        {
            // create path to registry location
            string regPath = GetRegPath(fileType, shellKeyName);

            // add context menu to the registry
            using (RegistryKey key =
                    Microsoft.Win32.Registry.ClassesRoot.CreateSubKey(regPath))
            {
                key.SetValue(null, menuText);
            }

            // add command that is invoked to the registry
            using (RegistryKey key = Microsoft.Win32.Registry.ClassesRoot.CreateSubKey($@"{regPath}\command"))
            {
                key.SetValue(null, menuCommand);
            }

            if (iconPath != null)
            {
                using (RegistryKey key = Microsoft.Win32.Registry.ClassesRoot.CreateSubKey($@"{regPath}"))
                {
                    key.SetValue("icon", iconPath);
                }
            }
        }

        public void UnregisterFileContextMenuEntry(string fileType, string shellKeyName)
        {
            if (string.IsNullOrEmpty(fileType))
                throw new ArgumentException(nameof(fileType));
            if (string.IsNullOrEmpty(shellKeyName))
                throw new ArgumentException(nameof(shellKeyName));

            // path to the registry location
            string regPath = GetRegPath(fileType, shellKeyName);

            // remove context menu from the registry
            Microsoft.Win32.Registry.ClassesRoot.DeleteSubKeyTree(regPath);
        }

        public bool IsFileContextMenuEntryRegistered(string fileType, string shellKeyName)
        {
            string regPath = GetRegPath(fileType, shellKeyName);

            var key = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(regPath);

            return key != null;
        }
    }
}

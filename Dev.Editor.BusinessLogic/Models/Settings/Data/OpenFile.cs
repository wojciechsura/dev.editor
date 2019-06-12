using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.Settings.Data
{
    public class OpenFile
    {
        public string Path { get; set; }
        public bool VirtualPath { get; set; }
        public bool Modified { get; set; }
        public string StoredPath { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.DuplicatedLines
{
    public class DuplicatedLinesFinderConfigModel
    {
        public DuplicatedLinesFinderConfigModel(string defaultFolder)
        {
            DefaultFolder = defaultFolder;
        }

        public string DefaultFolder { get; }
    }
}

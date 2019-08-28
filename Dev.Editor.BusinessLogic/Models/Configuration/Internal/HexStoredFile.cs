using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.Configuration.Internal
{
    public class HexStoredFile : BaseStoredFile
    {
        internal const string NAME = "HexStoredFile";

        public HexStoredFile()
            : base(NAME)
        {

        }
    }
}

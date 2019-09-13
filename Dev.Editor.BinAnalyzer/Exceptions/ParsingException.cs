using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BinAnalyzer.Exceptions
{
    public class ParsingException : Exception
    {
        public ParsingException(string message)
            : base(message)
        {

        }
    }
}

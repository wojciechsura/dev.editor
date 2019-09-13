using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BinAnalyzer.Exceptions
{
    public class SyntaxErrorException : Exception
    {
        public SyntaxErrorException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}

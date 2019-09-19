using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BinAnalyzer.Exceptions
{
    public class BaseSourceReferenceException : BaseLocalizedException
    {
        public BaseSourceReferenceException(int line, int column, string message, string localizedMessage)
            : base(message, localizedMessage)
        {
            Line = line;
            Column = column;
        }

        public int Line { get; }
        public int Column { get; }
    }
}

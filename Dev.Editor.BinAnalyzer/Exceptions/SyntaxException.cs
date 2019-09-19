using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BinAnalyzer.Exceptions
{
    public class SyntaxException : BaseSourceReferenceException
    {
        public SyntaxException(int line, int column, string message, string localizedErrorMessage) 
            : base(line, column, message, localizedErrorMessage)
        {

        }
    }
}

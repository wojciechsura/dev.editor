using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BinAnalyzer.Exceptions
{
    public class AnalysisException : BaseSourceReferenceException
    {
        public AnalysisException(int line, int column, string message, string localizedMessage) 
            : base(line, column, message, localizedMessage)
        {

        }
    }
}

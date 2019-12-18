using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BinAnalyzer.Exceptions
{
    public class BaseLocalizedException : Exception
    {
        public BaseLocalizedException(string message, string localizedErrorMessage)
            : base(message)
        {
            LocalizedErrorMessage = localizedErrorMessage;
        }

        public virtual string LocalizedErrorMessage { get; }
    }
}

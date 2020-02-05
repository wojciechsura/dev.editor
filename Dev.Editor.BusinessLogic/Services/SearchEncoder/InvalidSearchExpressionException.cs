using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Services.SearchEncoder
{

    [Serializable]
    public class InvalidSearchExpressionException : Exception
    {
        public InvalidSearchExpressionException() { }
        public InvalidSearchExpressionException(string message) : base(message) { }
        public InvalidSearchExpressionException(string message, Exception inner) : base(message, inner) { }
        protected InvalidSearchExpressionException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}

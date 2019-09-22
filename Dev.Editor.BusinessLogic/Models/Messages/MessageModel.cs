using Dev.Editor.BusinessLogic.Types.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.Messages
{
    public class MessageModel
    {
        public MessageModel(string message, MessageSeverity severity, string path = null, int? line = null, int? column = null, string code = null)
        {
            Message = message;
            Severity = severity;
            Path = path;
            Line = line;
            Column = column;
            Code = code;
        }

        public MessageSeverity Severity { get; }
        public string Code { get; }
        public string Message { get; }
        public string Path { get; }
        public string Filename => System.IO.Path.GetFileName(Path);
        public int? Line { get; }
        public int? Column { get; }

        public int? DisplayRow => Line != null ? Line + 1 : null;
        public int? DisplayCol => Column != null ? Column + 1 : null;
    }
}

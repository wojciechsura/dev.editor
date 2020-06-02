using Dev.Editor.BusinessLogic.Models.TextTransform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Services.TextTransform
{
    public interface ITextTransformService
    {
        string Escape(string input, EscapeConfig config);
        string Unescape(string input, EscapeConfig config);
    }
}

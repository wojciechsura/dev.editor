using Dev.Editor.Resources;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.ViewModels.Main
{
    public partial class MainWindowViewModel
    {
        public void DoFormatJson()
        {
            TransformLines(text =>
            {
                try
                {
                    using (var stringReader = new StringReader(text))
                    using (var stringWriter = new StringWriter())
                    {
                        var jsonReader = new JsonTextReader(stringReader);
                        var jsonWriter = new JsonTextWriter(stringWriter) { Formatting = Formatting.Indented };
                        jsonWriter.WriteToken(jsonReader);
                        return (true, stringWriter.ToString());
                    }
                }
                catch
                {
                    messagingService.ShowError(Strings.Message_CannotFormatInvalidJson);

                    return (false, null);
                }
            });
        }
    }
}

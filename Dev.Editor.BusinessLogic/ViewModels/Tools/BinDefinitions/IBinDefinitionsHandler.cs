using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dev.Editor.BusinessLogic.Models.Configuration.BinDefinitions;
using Dev.Editor.BusinessLogic.ViewModels.Tools.Base;

namespace Dev.Editor.BusinessLogic.ViewModels.Tools.BinDefinitions
{
    public interface IBinDefinitionsHandler : IToolHandler
    {
        void NewTextDocument(string text);
        void OpenTextFile(string path);
        void RequestOpenBinFile(BinDefinition binDefinition);
    }
}

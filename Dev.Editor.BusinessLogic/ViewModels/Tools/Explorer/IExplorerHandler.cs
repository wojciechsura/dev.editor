using Dev.Editor.BusinessLogic.ViewModels.Tools.Base;
using Dev.Editor.Common.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.ViewModels.Tools.Explorer
{
    public interface IExplorerHandler : IToolHandler
    {
        void OpenTextFile(string path);

        string GetCurrentDocumentPath();

        BaseCondition CurrentDocumentHasPathCondition { get; }
    }
}

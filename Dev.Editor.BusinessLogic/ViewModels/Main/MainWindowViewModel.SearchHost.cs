using Dev.Editor.BusinessLogic.Models.Search;
using Dev.Editor.BusinessLogic.ViewModels.Search;
using Dev.Editor.Common.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Dev.Editor.BusinessLogic.ViewModels.Main
{
    public partial class MainWindowViewModel : ISearchHost
    {
        public void FindNext(SearchModel searchModel)
        {
            (int selStart, int selLength) = activeDocument.GetSelection();

            int start = searchModel.SearchBackwards ? selStart : selStart + selLength;

            Match match = searchModel.Regex.Match(activeDocument.Document.Text, start);

            if (!match.Success)  // start again from beginning or end
            {
                if (searchModel.SearchBackwards)
                    match = searchModel.Regex.Match(activeDocument.Document.Text, activeDocument.Document.Text.Length);
                else
                    match = searchModel.Regex.Match(activeDocument.Document.Text, 0);
            }

            if (match.Success)
                activeDocument.SetSelection(match.Index, match.Length, true);
            else
                messagingService.Inform(Properties.Resources.Message_NoMorePatternsFound);
        }

        public void Replace(ReplaceModel replaceModel)
        {
			// TODO
        }

        public void ReplaceAll(ReplaceModel replaceModel)
        {
			// TODO
        }

        public BaseCondition CanSearchCondition => documentExistsCondition;
    }
}

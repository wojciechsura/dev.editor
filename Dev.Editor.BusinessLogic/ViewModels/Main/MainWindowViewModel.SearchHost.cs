﻿using Dev.Editor.BusinessLogic.Models.Search;
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
        private void InternalFindNext(SearchModel searchModel)
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
                messagingService.Inform(Resources.Strings.Message_NoMorePatternsFound);
        }

        public void FindNext(SearchModel searchModel)
        {
            activeDocument.LastSearch = searchModel;
            InternalFindNext(searchModel);
        }

        public void Replace(ReplaceModel replaceModel)
        {            
            string input = activeDocument.GetSelectedText();

            Match match = replaceModel.Regex.Match(input);

            if (match.Success && match.Index == 0 && match.Length == input.Length)
            {
                (int selStart, int selLength) = activeDocument.GetSelection();
                activeDocument.Document.Replace(selStart, selLength, replaceModel.Replace);
            }

            FindNext(replaceModel);
        }

        public void ReplaceAll(ReplaceModel replaceModel)
        {
            activeDocument.LastSearch = replaceModel;

            int offset = 0;
            activeDocument.RunAsSingleHistoryEntry(() =>
            {
                foreach (Match match in replaceModel.Regex.Matches(activeDocument.Document.Text))
                {
                    activeDocument.Document.Replace(offset + match.Index, match.Length, replaceModel.Replace);
                    offset += replaceModel.Replace.Length - match.Length;
                }
            });
        }

        public BaseCondition CanSearchCondition => documentExistsCondition;
    }
}

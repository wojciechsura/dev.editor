using Dev.Editor.BusinessLogic.Models.Search;
using Dev.Editor.BusinessLogic.ViewModels.Search;
using Dev.Editor.Common.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Dev.Editor.BusinessLogic.ViewModels.Document;
using Dev.Editor.Resources;

namespace Dev.Editor.BusinessLogic.ViewModels.Main
{
    public partial class MainWindowViewModel : ISearchHost
    {
        BaseCondition ISearchHost.CanSearchCondition => documentExistsCondition;

        BaseCondition ISearchHost.SelectionAvailableCondition => regularSelectionAvailableCondition;

        private void InternalFindNext(SearchModel searchModel)
        {
            var document = (TextDocumentViewModel)activeDocument;

            (int selStart, int selLength) = document.GetSelection();

            int start = searchModel.SearchBackwards ? selStart : selStart + selLength;

            Match match = searchModel.Regex.Match(document.Document.Text, start);

            if (!match.Success)  // start again from beginning or end
            {
                if (searchModel.SearchBackwards)
                    match = searchModel.Regex.Match(document.Document.Text, document.Document.Text.Length);
                else
                    match = searchModel.Regex.Match(document.Document.Text, 0);
            }

            if (match.Success)
                document.SetSelection(match.Index, match.Length, true);
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
            var document = (TextDocumentViewModel)activeDocument;

            string input = document.GetSelectedText();

            Match match = replaceModel.Regex.Match(input);

            if (match.Success && match.Index == 0 && match.Length == input.Length)
            {
                (int selStart, int selLength) = document.GetSelection();

                if (replaceModel.IsRegexReplace)
                {
                    var newReplace = replaceModel.Regex.Replace(match.Value, replaceModel.Replace);
                    document.Document.Replace(selStart, selLength, newReplace);
                }
                else
                {
                    document.Document.Replace(selStart, selLength, replaceModel.Replace);
                }
            }

            FindNext(replaceModel);
        }

        public void ReplaceAll(ReplaceAllModel replaceModel)
        {
            var document = (TextDocumentViewModel)activeDocument;

            document.LastSearch = replaceModel;

            int replaceCount = 0;

            if (!replaceModel.InSelection)
            {
                int offset = 0;
                
                replaceCount = document.RunAsSingleHistoryEntry(() =>
                {
                    MatchCollection matches = replaceModel.Regex.Matches(document.Document.Text);

                    foreach (Match match in matches)
                    {
                        if (replaceModel.IsRegexReplace)
                        {
                            var newReplace = replaceModel.Regex.Replace(match.Value, replaceModel.Replace);
                            document.Document.Replace(offset + match.Index, match.Length, newReplace);
                            offset += newReplace.Length - match.Length;
                        }
                        else
                        {
                            document.Document.Replace(offset + match.Index, match.Length, replaceModel.Replace);
                            offset += replaceModel.Replace.Length - match.Length;
                        }
                    }

                    return matches.Count;
                });
            }
            else
            {
                (int selStart, int selLen) = document.GetSelection();
                string selection = document.GetSelectedText();

                int offset = 0;

                replaceCount = document.RunAsSingleHistoryEntry(() =>
                {
                    MatchCollection matches = replaceModel.Regex.Matches(selection);                    

                    foreach (Match match in matches)
                    {
                        document.Document.Replace(offset + selStart + match.Index, match.Length, replaceModel.Replace);
                        offset += replaceModel.Replace.Length - match.Length;
                    }

                    return matches.Count;
                });
            }

            if (replaceCount > 0)
            {
                messagingService.Inform(String.Format(Strings.Message_ReplacedOccurrences, replaceCount));
            }
            else
            {
                messagingService.Inform(Strings.Message_NoMorePatternsFound);
            }
        }
    }
}

using Dev.Editor.BusinessLogic.Models.Search;
using Dev.Editor.BusinessLogic.Types.Search;
using Dev.Editor.Common.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Services.SearchEncoder
{
    class SearchEncoderService : ISearchEncoderService
    {
        private const string EolFixString = "\r?";

        /// <summary>
        /// Replaces $ signs in the pattern with \r?$
        /// Takes into account, that $ signs in the pattern
        /// may be escaped. Handles properly situations, that
        /// might effect in false positives (such as "\$" in "\\$").
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        private string FixDotNetEoLRegex(string pattern)
        {
            int startIndex = 0;

            while (startIndex < pattern.Length)
            {
                var res = pattern.IndexOf('$', startIndex);
                if (res == -1)
                {
                    // No more $'s
                    break;
                }

                // Check escaping
                int backslashCount = 0;
                for (int i = res - 1; i >= 0; i--)
                {
                    if (pattern[i] == '\\')
                        backslashCount++;
                    else
                        break;
                }

                if (backslashCount % 2 == 1)
                {
                    // Odd count of backslashes - $ is escaped
                    startIndex = res + 1;
                    continue;
                }

                // Add \r? before the $ sign
                pattern.Insert(res, EolFixString);
                res += EolFixString.Length;

                startIndex = res + 1;
            }

            return pattern;
        }

        private string GetReplaceText(string replace, SearchMode searchMode)
        {
            replace = replace ?? String.Empty;

            switch (searchMode)
            {
                case SearchMode.Normal:
                    return replace;
                case SearchMode.Extended:
                    return replace.Unescape();
                case SearchMode.RegularExpressions:
                    return replace;
                default:
                    throw new InvalidOperationException("Unsupported search mode!");
            }
        }

        private Regex GetSearchRegex(string textToFind,
            bool searchBackwards,
            bool caseSensitive,
            bool wholeWordsOnly,
            SearchMode searchMode)
        {
            RegexOptions options = RegexOptions.None;
            if (searchBackwards)
                options |= RegexOptions.RightToLeft;
            if (!caseSensitive)
                options |= RegexOptions.IgnoreCase;
            options |= RegexOptions.Multiline;

            switch (searchMode)
            {
                case SearchMode.RegularExpressions:
                    {
                        string pattern = textToFind;

                        // See: https://docs.microsoft.com/en-us/dotnet/standard/base-types/anchors-in-regular-expressions#end-of-string-or-line-
                        pattern = FixDotNetEoLRegex(pattern);

                        return new Regex(pattern, options);
                    }
                case SearchMode.Extended:
                    {
                        string pattern = Regex.Escape(textToFind.Unescape());

                        if (wholeWordsOnly)
                            pattern = "\\b" + pattern + "\\b";
                        return new Regex(pattern, options);
                    }
                case SearchMode.Normal:
                    {
                        string pattern = Regex.Escape(textToFind);
                        if (wholeWordsOnly)
                            pattern = "\\b" + pattern + "\\b";
                        return new Regex(pattern, options);
                    }

                default:
                    throw new InvalidOperationException("Unsupported search mode!");
            }
        }

        public SearchReplaceModel SearchDescriptionToModel(SearchReplaceDescription description)
        {
            Regex searchRegex;

            try
            {
                searchRegex = GetSearchRegex(description.Search,
                    description.IsSearchBackwards,
                    description.IsCaseSensitive,
                    description.IsWholeWordsOnly,
                    description.SearchMode);                
            }
            catch
            {
                throw new InvalidSearchExpressionException();
            }

            string replaceText = GetReplaceText(description.Replace, description.SearchMode);

            return new SearchReplaceModel(searchRegex,
                replaceText,
                description.IsSearchBackwards,
                description.SearchMode == SearchMode.RegularExpressions,
                description.IsReplaceAllInSelection);            
        }
    }
}

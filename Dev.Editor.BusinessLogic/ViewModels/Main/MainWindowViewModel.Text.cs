using Dev.Editor.BusinessLogic.Models.Dialogs;
using Dev.Editor.BusinessLogic.Models.TextTransform;
using Dev.Editor.BusinessLogic.ViewModels.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Dev.Editor.BusinessLogic.ViewModels.Main
{
    public partial class MainWindowViewModel
    {
        private void DoBase64Encode()
        {
            TransformText(text =>
            {
                if (string.IsNullOrEmpty(text))
                    return (false, null);

                var bytes = System.Text.Encoding.UTF8.GetBytes(text);
                return (true, System.Convert.ToBase64String(bytes));
            });
        }

        private void DoBase64Decode()
        {
            TransformText(text =>
            {
                if (string.IsNullOrEmpty(text))
                    return (false, null);

                try
                {
                    var bytes = System.Convert.FromBase64String(text);
                    return (true, System.Text.Encoding.UTF8.GetString(bytes));
                }
                catch
                {
                    messagingService.Warn(Resources.Strings.Message_InvalidBase64String);
                    return (false, null);
                }
            });
        }

        private void DoHtmlEntitiesDecode()
        {
            TransformText(text => (true, HttpUtility.HtmlDecode(text)));
        }

        private void DoHtmlEntitiesEncode()
        {
            TransformText(text => (true, HttpUtility.HtmlEncode(text)));
        }

        private void DoEscape(object parameter = null)
        {
            EscapeConfigResult model;

            if (parameter is string str && str == "\\")
            {
                PerformEscape(new EscapeConfigResult("\\", true, true, true));
            }
            else
            {
                bool result;
                (result, model) = dialogService.ShowEscapeConfigDialog(new EscapeConfigModel(true));

                if (result)
                    PerformEscape(model);
            }
        }

        private void PerformEscape(EscapeConfigResult model)
        {
            TransformText(text =>
            {
                try
                {
                    var transformed = textTransformService.Escape(text, new EscapeConfig(model.EscapeChar[0], model.IncludeSingleQuotes, model.IncludeDoubleQuotes, model.IncludeSpecialCharacters));
                    return (true, transformed);
                }
                catch
                {
                    messagingService.ShowError(Resources.Strings.Message_FailedToEscapeString);
                    return (false, null);
                }
            });
        }

        private void DoUnescape(object parameter = null)
        {
            if (parameter is string str && str == "\\")
            {
                PerformUnescape(new EscapeConfigResult("\\", true, true, true));
            }
            else
            {
                (bool result, EscapeConfigResult model) = dialogService.ShowEscapeConfigDialog(new EscapeConfigModel(false));

                if (result)
                    PerformUnescape(model);
            }
        }

        private void PerformUnescape(EscapeConfigResult model)
        {
            TransformText(text =>
            {
                try
                {
                    var transformed = textTransformService.Unescape(text, new EscapeConfig(model.EscapeChar[0], model.IncludeSingleQuotes, model.IncludeDoubleQuotes, model.IncludeSpecialCharacters));
                    return (true, transformed);
                }
                catch
                {
                    messagingService.ShowError(Resources.Strings.Message_FailedToUnescapeString);
                    return (false, null);
                }
            });
        }

        private void DoInvertCase()
        {
            TransformText(text =>
            {
                StringBuilder builder = new StringBuilder();

                foreach (var ch in text)
                {
                    if (char.IsUpper(ch))
                        builder.Append(char.ToLower(ch));
                    else if (char.IsLower(ch))
                        builder.Append(char.ToUpper(ch));
                    else
                        builder.Append(ch);
                }

                return (true, builder.ToString());
            });
        }

        private void DoSentenceCase()
        {
            TransformText(text =>
            {
                StringBuilder builder = new StringBuilder();
                bool isSentenceStart = true;

                foreach (var ch in text)
                {
                    if (char.IsLower(ch))
                    {
                        if (isSentenceStart)
                        {
                            builder.Append(char.ToUpper(ch));
                            isSentenceStart = false;
                        }
                        else
                            builder.Append(ch);
                    }
                    else if (char.IsUpper(ch))
                    {
                        if (isSentenceStart)
                        {
                            builder.Append(ch);
                            isSentenceStart = false;
                        }
                        else
                            builder.Append(char.ToLower(ch));
                    }
                    else if (ch == '.')
                    {
                        isSentenceStart = true;
                        builder.Append(ch);
                    }
                    else
                        builder.Append(ch);
                }

                return (true, builder.ToString());
            });
        }

        private void DoNamingCase()
        {
            TransformText(text =>
            {
                StringBuilder builder = new StringBuilder();
                bool isInName = false;

                foreach (var ch in text)
                {
                    if (char.IsLetter(ch))
                    {
                        if (!isInName)
                        {
                            isInName = true;
                            builder.Append(char.ToUpper(ch));
                        }
                        else
                        {
                            builder.Append(char.ToLower(ch));
                        }
                    }
                    else
                    {
                        isInName = false;
                        builder.Append(ch);
                    }                    
                }

                return (true, builder.ToString());
            });
        }

        private void DoUppercase()
        {
            TransformText(text =>
            {
                return (true, text.ToUpper());
            });
        }

        private void DoLowercase()
        {
            TransformText(text =>
            {
                return (true, text.ToLower());
            });
        }
    }
}

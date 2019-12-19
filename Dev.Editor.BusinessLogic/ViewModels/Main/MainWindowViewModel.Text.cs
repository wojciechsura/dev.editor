using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.ViewModels.Main
{
    public partial class MainWindowViewModel
    {
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

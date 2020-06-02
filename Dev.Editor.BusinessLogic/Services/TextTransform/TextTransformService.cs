using Dev.Editor.BusinessLogic.Models.TextTransform;
using Dev.Editor.Common.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Services.TextTransform
{
    class TextTransformService : ITextTransformService
    {
        public string Unescape(string input, EscapeConfig config)
        {
            StringBuilder sb = new StringBuilder();

            int pos = 0;
            while (pos < input.Length)
            {
                if (input[pos] == config.EscapeChar)
                {
                    if (++pos >= input.Length)
                        throw new ArgumentException("Invalid escape sequence!");

                    switch (input[pos])
                    {
                        case '0':
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                            sb.Append(input.ScanOctal(ref pos));
                            continue;
                        case 'x':
                            sb.Append(input.ScanHex(ref pos, 2));
                            continue;
                        case 'u':
                            sb.Append(input.ScanHex(ref pos, 4));
                            continue;
                        case 'a':
                            sb.Append('\u0007');
                            pos++;
                            continue;
                        case 'b':
                            sb.Append('\b');
                            pos++;
                            continue;
                        case 'e':
                            sb.Append('\u001B');
                            pos++;
                            continue;
                        case 'f':
                            sb.Append('\f');
                            pos++;
                            continue;
                        case 'n':
                            sb.Append('\n');
                            pos++;
                            continue;
                        case 'r':
                            sb.Append('\r');
                            pos++;
                            continue;
                        case 't':
                            sb.Append('\t');
                            pos++;
                            continue;
                        case 'v':
                            sb.Append('\u000B');
                            pos++;
                            continue;
                        case 'c':
                            sb.Append(input.ScanControl(ref pos));
                            continue;
                        default:
                            sb.Append(input[pos]);
                            pos++;
                            continue;                            
                    }
                }
                else
                {
                    sb.Append(input[pos]);
                    pos++;
                }
            }

            return sb.ToString();
        }

        public string Escape(string input, EscapeConfig config)
        {
            if (input == null)
                return null;

            StringBuilder result = new StringBuilder();

            int pos = 0;
            while (pos < input.Length)
            {
                switch (input[pos])
                {
                    case '\a':
                        if (!config.IncludeSpecialCharacters)
                            goto default;
                        result.Append($"{config.EscapeChar}a");
                        break;
                    case '\b':
                        if (!config.IncludeSpecialCharacters)
                            goto default;
                        result.Append($"{config.EscapeChar}b");
                        break;
                    case '\f':
                        if (!config.IncludeSpecialCharacters)
                            goto default;
                        result.Append($"{config.EscapeChar}f");
                        break;
                    case '\n':
                        if (!config.IncludeSpecialCharacters)
                            goto default;
                        result.Append($"{config.EscapeChar}n");
                        break;
                    case '\r':
                        if (!config.IncludeSpecialCharacters)
                            goto default;
                        result.Append($"{config.EscapeChar}r");
                        break;
                    case '\t':
                        if (!config.IncludeSpecialCharacters)
                            goto default;
                        result.Append($"{config.EscapeChar}t");
                        break;
                    case '\v':
                        if (!config.IncludeSpecialCharacters)
                            goto default;
                        result.Append($"{config.EscapeChar}v");
                        break;
                    case '\'':
                        if (!config.IncludeSingleQuotes)
                            goto default;
                        result.Append($"{config.EscapeChar}'");
                        break;
                    case '\"':
                        if (!config.IncludeDoubleQuotes)
                            goto default;
                        result.Append($"{config.EscapeChar}\"");
                        break;
                    default:
                        if (input[pos] == config.EscapeChar)
                        {
                            result.Append($"{config.EscapeChar}{config.EscapeChar}");
                        }
                        else if (config.IncludeSpecialCharacters && (input[pos] < 32 || input[pos] > 127))
                        {
                            result.Append($"{config.EscapeChar}x{(int)input[pos]:X4}");
                        }
                        else
                        {
                            result.Append(input[pos]);
                        }
                        break;
                }

                pos++;
            }

            return result.ToString();
        }
    }
}

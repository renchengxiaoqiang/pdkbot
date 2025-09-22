using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Text.RegularExpressions;
using PdkBot.BotLib.Misc;

namespace PdkBot.BotLib.Extensions
{
    public static class StringEx
    {
        private static string _integerPt = "^[0-9]+$";
        private static string _numberPt = "^[0-9+-eE.]$";
        private const string _keyConnector = "!@^$";

        public static string xToBanJiao(this string input)
        {
            var sb = new StringBuilder(input);
            for (int i = 0; i < input.Length; i++)
            {
                var c = input[i];
                if (c == 12288)
                {
                    sb[i] = (char)32;
                }
                else if (c > 65280 && c < 65375)
                {
                    sb[i] = (char)(c - 65248);
                }
            }
            return sb.ToString();
        }


        public static string xRemoveLineBreak(this string input, string rep = " ")
        {
            if (input != null)
            {
                input = input.Replace("\r\n", rep);
                input = input.Replace("\n", rep);
                input = input.Replace("\r", rep);
            }
            return input;
        }

        public static string xRemoveSpace(this string input)
        {
            if (input == null) return null;
            var sb = new StringBuilder();
            foreach (char c in input)
            {
                if (c > ' ')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static string xRemoveTab(this string input)
        {
            if (input == null) return null;
            input = input.Replace("\t", "");
            while (input.IndexOf("  ") >= 0)
            {
                input = input.Replace("  ", " ");
            }
            return input;
        }

        public static string xGenGuidB32Str()
        {
            return Base32Encoding.ToString(Guid.NewGuid().ToByteArray()).Substring(0, 26);
        }

        public static string xGenGuidB64Str()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Replace("/", "_").Replace("+", "-").Substring(0, 22);
        }

        public static int xLengthOfLeftEndSameString(this string txt, string another)
        {
            int sameLen = 0;
            if (!string.IsNullOrEmpty(txt) && !string.IsNullOrEmpty(another))
            {
                int length = Math.Min(txt.Length, another.Length);
                while (sameLen < length && txt[sameLen] == another[sameLen])
                {
                    sameLen++;
                }
            }
            return sameLen;
        }

        public static bool xIsUrl(this string input)
        {
            if (Uri.IsWellFormedUriString(input, UriKind.Absolute)) return true;

            string uriString = "http://" + input;
            if (Uri.IsWellFormedUriString(uriString, UriKind.Absolute))
            {
                Uri uri = new Uri(uriString);
                return Regex.IsMatch(uri.Host.ToLower(), ".*\\.(com|net|cn|org|gov|edu|cc|top|club|site|shop|wang|ltd|red|mobi|info|biz|pro|vip|ren|xin)$");
            }

            return false;
        }

        public static bool xIsNullOrEmptyOrSpace(this string s)
        {
            return s == null || s == "" || s.Trim() == "";
        }

        public static string xSlice(this string txt, int skipLeft, int skipRight)
        {
            if (string.IsNullOrEmpty(txt)) return txt;
            return txt.Length <= skipLeft + skipRight ? string.Empty : txt.Substring(skipLeft, txt.Length - skipRight - skipLeft);
        }

        public static string[] xSplitByLine(this string text, StringSplitOptions opt = StringSplitOptions.RemoveEmptyEntries)
        {
            return text.Split(new string[]
			{
				Environment.NewLine
			}, opt);
        }

        public static string xRemoveLastChar(this string txt)
        {
            if (!string.IsNullOrEmpty(txt))
            {
                txt = txt.Substring(0, txt.Length - 1);
            }
            return txt;
        }


        public static string xAppendIfNotEndWith(this string main, string tail)
        {
            return !string.IsNullOrEmpty(tail) && !main.EndsWith(tail) ? main + tail : main;
        }

        public static string xTrimIfEndWith(this string main, string tail)
        {
            return main.EndsWith(tail) ? main.Substring(0, main.Length - tail.Length) : main;
        }

        public static string xConvertChineseNumberStringToArabNumberString(this string input)
        {
            var sb = new StringBuilder();
            foreach (char c in input)
            {
                sb.Append(c.ChineseCharToArabChar());
            }
            return sb.ToString();
        }

        public static bool xNotEscaped(this string main, int idx, char escape = '\\')
        {
            int num = 0;
            idx--;
            while (idx >= 0 && main[idx] == escape)
            {
                num++;
                idx--;
            }
            return num % 2 == 0;
        }


        public static string xLimitCharCountPerLine(this string input, int maxCharPerLine)
        {
            string[] texts = input.xSplitByLine(StringSplitOptions.RemoveEmptyEntries);
            var list = new List<string>();
            foreach (string text in texts)
            {
                var tmp = text;
                while (tmp.Length >= maxCharPerLine)
                {
                    list.Add(tmp.Substring(0, maxCharPerLine));
                    tmp = tmp.Substring(maxCharPerLine);
                }
                if (tmp.Length > 0)
                {
                    list.Add(tmp);
                }
            }
            return list.xToString("\r\n", true);
        }

        public static string[] xSplitBySpace(this string text, StringSplitOptions opt = StringSplitOptions.RemoveEmptyEntries)
        {
            return text.Split(new char[] { ' ', '\u3000' }, opt);
        }

        public static string xRemoveSpaceAndPunctuationChars(this string input)
        {
            if (input == null) return null;

            var sb = new StringBuilder();
            foreach (char c in input)
            {
                if (c > ' ' && !char.IsPunctuation(c))
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static string xToBanJiaoAndRemoveCharThatAsciiValueLessThan32AndToLower(this string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            var sb = new StringBuilder(input.Length);
            foreach (char c in input)
            {
                char tmp = c;
                if (c >= 'A' && c <= 'Z')
                {
                    tmp = (char)(c + ' ');
                }
                else if (c > '＀' && c < '｟')
                {
                    tmp = (char)(c - 'ﻠ');
                }
                else if (c == '\u3000')
                {
                    tmp = ' ';
                }

                if (tmp > ' ')
                {
                    sb.Append(tmp);
                }
            }
            return sb.ToString();
        }

        public static string[] xSplitByComma(this string text, StringSplitOptions opt = StringSplitOptions.RemoveEmptyEntries)
        {
            return text.Split(new char[] { ',', '，' }, opt);
        }

        public static string xRemoveUselessZeroAfterPoint(this string numtxt)
        {
            if (numtxt != null && numtxt.Contains("."))
            {
                while (numtxt.Length > 2)
                {
                    char c = numtxt[numtxt.Length - 1];
                    if (c != '0' && c != '.')
                    {
                        break;
                    }
                    numtxt = numtxt.Substring(0, numtxt.Length - 1);
                    if (c == '.')
                    {
                        break;
                    }
                }
            }
            return numtxt;
        }

        public static bool xIsAllNumberChar(this string s)
        {
            if (string.IsNullOrEmpty(s)) return false;
            s = s.Trim();
            if (s.Length <= 0) return false;

            bool rt = true;
            foreach (char c in s)
            {
                if (!char.IsNumber(c))
                {
                    rt = false;
                    break;
                }
            }
            return rt;
        }

        public static string xToFileName(this string txt, char replace = '_')
        {
            return txt.Replace('\\', replace).Replace('/', replace).Replace(':', replace).Replace('*', replace).Replace('?', replace).Replace('"', replace).Replace('<', replace).Replace('>', replace).Replace('|', replace);
        }

        public static string xToBanJiaoAndRemoveEndingPunctuationsSpaces(this string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                input = input.xToBanJiao().Trim();
                while (input.Length > 0)
                {
                    char c = input[input.Length - 1];
                    if (!char.IsPunctuation(c) && c != ' ')
                    {
                        break;
                    }
                    input = input.Substring(0, input.Length - 1);
                }
            }
            return input;
        }

        public static bool xIsInteger(this string s)
        {
            return Regex.IsMatch(s ?? "", _integerPt);
        }
    }
}

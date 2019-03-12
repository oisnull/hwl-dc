using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace ConsoleApplication1.Models.Rule
{
    public class RuleModel
    {
        public string DefaultValue { get; set; }
        public XpathExtractModel XpathExtractModel { get; set; }
        public FilterRemoveChar RemoveChar { get; set; }
        public List<FilterReplaceChar> ReplaceChars { get; set; }

        public bool IsFilter()
        {
            return this.IsRemove() || this.IsReplace() || this.IsDefault();
        }

        public bool IsRemove()
        {
            return this.RemoveChar != null;
        }

        public bool IsReplace()
        {
            return this.ReplaceChars != null && this.ReplaceChars.Count > 0;
        }

        public bool IsDefault()
        {
            return this.DefaultValue != null;
        }

        public string Filter(string content)
        {
            if (this.IsDefault())
            {
                return this.DefaultValue;
            }

            if (this.IsRemove())
            {
                content = FilterUtil.Remove(content, this.RemoveChar);
            }

            if (this.IsReplace())
            {
                content = FilterUtil.Replace(content, this.ReplaceChars);
            }

            return content;
        }

        public List<string> Filter(List<string> contents)
        {
            if (this.IsDefault())
            {
                return new List<string>() { this.DefaultValue };
            }

            if (this.IsRemove())
            {
                contents = FilterUtil.Remove(contents, this.RemoveChar);
            }

            if (this.IsReplace())
            {
                contents = FilterUtil.Replace(contents, this.ReplaceChars);
            }

            return contents;
        }
    }

    public class FilterRemoveChar
    {
        public bool IsRegex { get; set; }
        public List<string> Chars { get; set; }
    }

    public class FilterReplaceChar
    {
        public bool IsRegex { get; set; }
        public string OldChar { get; set; }
        public string NewChar { get; set; }
    }

    public class FilterUtil
    {
        public static string Remove(string content, FilterRemoveChar removeChar)
        {
            if (removeChar == null || removeChar.Chars == null || removeChar.Chars.Count <= 0) return content;
            if (string.IsNullOrEmpty(content)) return null;

            removeChar.Chars.ForEach(f =>
            {
                if (removeChar.IsRegex)
                {
                    content = Regex.Replace(content, f, "");
                }
                else
                {
                    content = content.Replace(f, "");
                }
            });

            return content;
        }

        public static List<string> Remove(List<string> contents, FilterRemoveChar removeChar)
        {
            if (removeChar == null || removeChar.Chars == null || removeChar.Chars.Count <= 0) return contents;
            if (contents == null || contents.Count <= 0) return null;

            List<string> results = new List<string>(contents.Count);
            string res = string.Empty;
            foreach (string con in contents)
            {
                removeChar.Chars.ForEach(f =>
                {
                    if (removeChar.IsRegex)
                    {
                        res = Regex.Replace(con, f, "");
                    }
                    else
                    {
                        res = con.Replace(f, "");
                    }
                });
                results.Add(res);
            }

            return results;
        }

        public static string Replace(string content, List<FilterReplaceChar> replaceChars)
        {
            if (replaceChars == null || replaceChars.Count <= 0) return content;
            if (string.IsNullOrEmpty(content)) return null;

            replaceChars.ForEach(f =>
            {
                if (f.IsRegex)
                {
                    content = Regex.Replace(content, f.OldChar, f.NewChar);
                }
                else
                {
                    content = content.Replace(f.OldChar, f.NewChar);
                }
            });

            return content;
        }

        public static List<string> Replace(List<string> contents, List<FilterReplaceChar> replaceChars)
        {
            if (replaceChars == null || replaceChars.Count <= 0) return contents;
            if (contents == null || contents.Count <= 0) return null;

            List<string> results = new List<string>(contents.Count);
            string res = string.Empty;
            foreach (string con in contents)
            {
                replaceChars.ForEach(f =>
                {
                    if (f.IsRegex)
                    {
                        res = Regex.Replace(con, f.OldChar, f.NewChar);
                    }
                    else
                    {
                        res = con.Replace(f.OldChar, f.NewChar);
                    }
                });
                results.Add(res);
            }

            return results;
        }
    }

    public enum ExtractType
    {
        Text,
        Html
    }

    public class XpathExtractModel
    {
        public string XpathRule { get; set; }
        public List<string> XpathEndAttributes { get; set; }
        public ExtractType ExtractType { get; set; } = ExtractType.Html;
    }
}
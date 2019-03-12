using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApplication1.Models.Rule;

namespace ConsoleApplication1.Models.Coll
{
    public class CNNArticleRule : IArticleRule
    {
        public RuleModel GetAuthorRule()
        {
            return new RuleModel()
            {
                XpathExtractModel = new XpathExtractModel()
                {
                    XpathRule = "//span[@class='metadata__byline__author']"
                }
            };
        }

        public RuleModel GetContentRule()
        {
            return new RuleModel()
            {
                XpathExtractModel = new XpathExtractModel()
                {
                    XpathRule = "//section[@id='body-text']",
                },
                RemoveChar = new FilterRemoveChar()
                {
                    IsRegex = true,
                    Chars = new List<string>() { @"<[a|A]\s*[^>]*>(.*?)</[a|A]>", @"<cite\s*[^>]*>(.*?)</cite>", "<div class=\"zn-body__read-more-outbrain\">.*</div>" }
                }
            };
        }

        public RuleModel GetImageUrlsRule()
        {
            return new RuleModel()
            {
                XpathExtractModel = new XpathExtractModel()
                {
                    XpathRule = "//section[@id='body-text']//img[@src]",
                    XpathEndAttributes = new List<string>() { "src" }
                }
            };
        }

        public RuleModel GetKeysRule()
        {
            return new RuleModel() { DefaultValue = "CNN NEW" };
        }

        public RuleModel GetPublishDateRule()
        {
            return new RuleModel()
            {
                XpathExtractModel = new XpathExtractModel()
                {
                    XpathRule = "//p[@class='update-time']",
                    ExtractType = ExtractType.Text
                }
            };
        }

        public RuleModel GetSummaryRule()
        {
            return new RuleModel()
            {
                XpathExtractModel = new XpathExtractModel()
                {
                    XpathRule = "//div[@class='el__leafmedia el__leafmedia--sourced-paragraph']",
                    ExtractType = ExtractType.Text
                },
                RemoveChar = new FilterRemoveChar()
                {
                    Chars = new List<string>() { "(CNN)" }
                }
            };
        }

        public RuleModel GetTitleRule()
        {
            return new RuleModel()
            {
                XpathExtractModel = new XpathExtractModel()
                {
                    XpathRule = "//h1[@class='pg-headline']",
                    ExtractType = ExtractType.Text
                }
            };
        }
    }
}

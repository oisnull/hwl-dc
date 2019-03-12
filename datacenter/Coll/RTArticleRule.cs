using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApplication1.Models.Rule;

namespace ConsoleApplication1.Models.Coll
{
    public class RTArticleRule : IArticleRule
    {
        public RuleModel GetAuthorRule()
        {
            return new RuleModel()
            {
                XpathExtractModel = new XpathExtractModel()
                {
                    XpathRule = "//div[@class='media__title media__title_arcticle']",
                    ExtractType = ExtractType.Text
                }
            };
        }

        public RuleModel GetContentRule()
        {
            return new RuleModel()
            {
                XpathExtractModel = new XpathExtractModel()
                {
                    XpathRule = "//div[@class='article__text text ']",
                },
                RemoveChar = new FilterRemoveChar()
                {
                    IsRegex = true,
                    Chars = new List<string>() { @"<[a|A]\s*[^>]*>(.*?)</[a|A]>", "<div class=\"read-more\">([\\s\\S]*)</div>", "<a[^>]+href=\"([^\"]*)\"[^>]*>([\\s\\S]*?)</a>" }
                }
            };
        }

        public RuleModel GetImageUrlsRule()
        {
            return new RuleModel()
            {
                XpathExtractModel = new XpathExtractModel()
                {
                    XpathRule = "//div[@class='article__cover']//img[@src]",
                    XpathEndAttributes = new List<string>() { "src" }
                }
            };
        }

        public RuleModel GetKeysRule()
        {
            return new RuleModel() { DefaultValue = "RT NEW" };
        }

        public RuleModel GetPublishDateRule()
        {
            return new RuleModel()
            {
                XpathExtractModel = new XpathExtractModel()
                {
                    XpathRule = "//span[@class='date date_article-header']",
                    ExtractType = ExtractType.Text
                },
                RemoveChar = new FilterRemoveChar()
                {
                    Chars = new List<string>() { "Published time:" }
                }
            };
        }

        public RuleModel GetSummaryRule()
        {
            return new RuleModel()
            {
                XpathExtractModel = new XpathExtractModel()
                {
                    XpathRule = "//div[@class='article__summary summary ']",
                    ExtractType = ExtractType.Text
                }
            };
        }

        public RuleModel GetTitleRule()
        {
            return new RuleModel()
            {
                XpathExtractModel = new XpathExtractModel()
                {
                    XpathRule = "//h1[@class='article__heading']",
                    ExtractType = ExtractType.Text
                }
            };
        }
    }
}

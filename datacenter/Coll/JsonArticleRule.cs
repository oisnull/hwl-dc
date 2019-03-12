using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApplication1.Models.Rule;

namespace ConsoleApplication1.Models.Coll
{
    public class JsonArticleRule : IArticleRule
    {
        private List<ExtractRuleConfig> _configs;
        public JsonArticleRule(List<ExtractRuleConfig> configs)
        {
            this._configs = configs;
        }

        public RuleModel GetAuthorRule()
        {
            return this._configs.Where(c => c.Key == "author").Select(c => c.Rule).FirstOrDefault();
        }

        public RuleModel GetContentRule()
        {
            return this._configs.Where(c => c.Key == "content").Select(c => c.Rule).FirstOrDefault();
        }

        public RuleModel GetImageUrlsRule()
        {
            return this._configs.Where(c => c.Key == "imageurls").Select(c => c.Rule).FirstOrDefault();
        }

        public RuleModel GetKeysRule()
        {
            return this._configs.Where(c => c.Key == "keys").Select(c => c.Rule).FirstOrDefault();
        }

        public RuleModel GetPublishDateRule()
        {
            return this._configs.Where(c => c.Key == "publishdate").Select(c => c.Rule).FirstOrDefault();
        }

        public RuleModel GetSummaryRule()
        {
            return this._configs.Where(c => c.Key == "summary").Select(c => c.Rule).FirstOrDefault();
        }

        public RuleModel GetTitleRule()
        {
            return this._configs.Where(c => c.Key == "title").Select(c => c.Rule).FirstOrDefault();
        }
    }
}

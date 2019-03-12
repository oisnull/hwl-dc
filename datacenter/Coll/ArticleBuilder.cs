using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ConsoleApplication1.Models.Rule;

namespace ConsoleApplication1.Models.Coll
{
    public class ArticleBuilder
    {
        private ContentExtract _contentExtract;

        public ArticleBuilder()
        {
            this._contentExtract = new ContentExtract();
        }

        public ArticleModel BuildModel(string url, string htmlContent, IArticleRule rule)
        {
            if (string.IsNullOrEmpty(htmlContent) || rule == null) return null;

            this._contentExtract.ExecuteHtml(htmlContent);
            if (!string.IsNullOrEmpty(this._contentExtract.GetExceptionMessage()))
            {
                throw new Exception(this._contentExtract.GetExceptionMessage());
            }

            return this.CreateModel(url, rule);
        }

        public ArticleModel BuildModel(string url, IArticleRule rule)
        {
            if (string.IsNullOrEmpty(url) || rule == null) return null;

            this._contentExtract.ExecuteUrl(url);
            if (!string.IsNullOrEmpty(this._contentExtract.GetExceptionMessage()))
            {
                throw new Exception(this._contentExtract.GetExceptionMessage());
            }

            return this.CreateModel(url, rule);
        }

        private ArticleModel CreateModel(string url, IArticleRule rule)
        {
            ArticleModel artModel = new ArticleModel();

            artModel.OriginUrl = url;
            artModel.Title = this.GetInfo(rule.GetTitleRule());
            artModel.Keys = this.GetInfo(rule.GetKeysRule());
            artModel.Author = this.GetInfo(rule.GetAuthorRule());
            artModel.Summary = this.GetInfo(rule.GetSummaryRule());
            artModel.ImageUrls = this.GetInfos(rule.GetImageUrlsRule());
            artModel.Content = this.GetInfo(rule.GetContentRule());
            artModel.PublishDate = this.GetInfo(rule.GetPublishDateRule());

            return artModel;
        }

        private string GetInfo(RuleModel rule)
        {
            if (rule == null) return null;

            if (rule.IsDefault()) return rule.DefaultValue;

            string content = this._contentExtract.ParseFirst(rule.XpathExtractModel);

            if (rule.IsFilter())
            {
                return rule.Filter(content);
            }

            return content;
        }

        private List<string> GetInfos(RuleModel rule)
        {
            if (rule == null) return null;

            if (rule.IsDefault()) return new List<string>() { rule.DefaultValue };

            List<string> contents = this._contentExtract.ParseList(rule.XpathExtractModel);

            if (rule.IsFilter())
            {
                return rule.Filter(contents);
            }

            return contents;
        }
    }

    public class ArticleModel
    {
        public string OriginUrl { get; set; }
        public string Title { get; set; }
        public string Keys { get; set; }
        public string Author { get; set; }
        public string Summary { get; set; }
        public List<string> ImageUrls { get; set; }
        public string Content { get; set; }
        public string PublishDate { get; set; }
    }

    public interface IArticleRule
    {
        RuleModel GetTitleRule();
        RuleModel GetKeysRule();
        RuleModel GetSummaryRule();
        RuleModel GetAuthorRule();
        RuleModel GetImageUrlsRule();
        RuleModel GetContentRule();
        RuleModel GetPublishDateRule();
    }

}

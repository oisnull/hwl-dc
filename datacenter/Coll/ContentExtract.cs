using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleApplication1.Models.Rule;

namespace ConsoleApplication1.Models.Coll
{
    public class ContentExtract
    {
        private string _url;
        private Encoding _encoding;

        private HtmlNode _rootNode;
        private string _exceptionMessage;

        public string GetExceptionMessage()
        {
            return this._exceptionMessage;
        }

        private void Init(string url, Encoding encoding)
        {
            this._rootNode = null;
            this._exceptionMessage = null;
            this._url = url;
            if (encoding == null) this._encoding = Encoding.UTF8;
            else this._encoding = encoding;

            if (string.IsNullOrEmpty(this._url)) throw new ArgumentNullException("url");
        }

        public void ExecuteUrl(string url, Encoding encoding = null)
        {
            string html = null;
            try
            {
                this.Init(url, encoding);

                html = Common.GetHtmlContent(url, this._encoding);
            }
            catch (Exception ex)
            {
                this._exceptionMessage = ex.Message;
            }

            this.ExecuteHtml(html);
        }

        public void ExecuteHtml(string htmlContent)
        {
            if (string.IsNullOrEmpty(htmlContent))
            {
                this._exceptionMessage = "Html content can be not empty.";
                return;
            }

            this._exceptionMessage = null;

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(htmlContent);
            this._rootNode = document.DocumentNode;
        }

        public string GetHtml()
        {
            if (this._rootNode == null) return null;

            return this._rootNode.InnerHtml;
        }

        public string GetText()
        {
            if (this._rootNode == null) return null;

            return this._rootNode.InnerText;
        }

        public List<string> ParseList(XpathExtractModel model)
        {
            if (this._rootNode == null || model == null || string.IsNullOrEmpty(model.XpathRule)) return null;

            HtmlNodeCollection nodes = this._rootNode.SelectNodes(model.XpathRule);

            return this.GetNodeValues(nodes, model.XpathEndAttributes, model.ExtractType);
        }

        private List<string> GetNodeValues(HtmlNodeCollection nodes, List<string> xpathEndAttributes, ExtractType extractType)
        {
            if (nodes == null || nodes.Count <= 0) return null;

            if (xpathEndAttributes != null && xpathEndAttributes.Count > 0)
            {
                return nodes.Select(n => n.Attributes.Where(a => xpathEndAttributes.Contains(a.Name)).Select(a => a.Value).FirstOrDefault()).
                    Where(n => !string.IsNullOrEmpty(n))
                    .ToList();
            }

            switch (extractType)
            {
                case ExtractType.Text:
                    return nodes.Select(n => n.InnerText.Trim()).ToList();
                case ExtractType.Html:
                default:
                    return nodes.Select(n => n.InnerHtml.Trim()).ToList();
            }
        }

        public string ParseFirst(XpathExtractModel model)
        {
            List<string> results = this.ParseList(model);
            if (results == null || results.Count <= 0) return null;

            return results.FirstOrDefault();

            //if (this._rootNode == null || model == null || string.IsNullOrEmpty(model.XpathRule)) return null;

            //HtmlNodeCollection nodes = this._rootNode.SelectNodes(model.XpathRule);
            //if (nodes == null) return null;

            //switch (model.ExtractType)
            //{
            //    case ExtractType.Text:
            //        return nodes.FirstOrDefault().InnerText.Trim();
            //    case ExtractType.Html:
            //    default:
            //        return nodes.FirstOrDefault().InnerHtml.Trim();
            //}
        }
    }
}

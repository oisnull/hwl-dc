using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1.Models.Coll
{
    public class UrlExtract
    {
        private string _url;
        private Encoding _encoding;

        private Uri _uri;
        private string _exceptionMessage;
        private string _htmlContent;

        public string GetExceptionMessage()
        {
            return this._exceptionMessage;
        }

        public string GetHtmlContent()
        {
            return this._htmlContent;
        }

        public Encoding GetEncoding()
        {
            return this._encoding;
        }

        private void Init(string url, Encoding encoding)
        {
            this._htmlContent = null;
            this._exceptionMessage = null;
            this._url = url;
            if (encoding == null) this._encoding = Encoding.UTF8;
            else this._encoding = encoding;

            this._uri = new Uri(this._url);
        }

        public List<string> Execute(string url, Encoding encoding = null)
        {
            if (string.IsNullOrEmpty(url)) return null;

            this.Init(url, encoding);

            try
            {
                this._htmlContent = Common.GetHtmlContent(url, this._encoding);
            }
            catch (Exception ex)
            {
                this._exceptionMessage = ex.Message;
            }

            return this.GetUrls(this._htmlContent);
        }

        public List<string> GetUrls(string htmlContent)
        {
            if (string.IsNullOrEmpty(htmlContent)) return null;

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(htmlContent);

            HtmlNode rootNode = document.DocumentNode;
            HtmlNodeCollection nodes = rootNode.SelectNodes("//a[@href]");
            if (nodes == null || nodes.Count <= 0) return null;

            List<string> urls = new List<string>();
            string hrefValue = string.Empty;
            foreach (HtmlNode item in nodes)
            {
                hrefValue = this.SupplyHref(item.GetAttributeValue("href", null));
                if (hrefValue == null) continue;

                urls.Add(hrefValue);
            }

            return urls.Distinct().ToList();
        }

        private string SupplyHref(string url)
        {
            if (string.IsNullOrEmpty(url)) return null;

            if (url == "/") return null;
            if (url.StartsWith("#")) return null;
            if (url.ToLower().StartsWith("javascript")) return null;

            if (url.StartsWith("/"))
            {
                return string.Format("{0}://{1}{2}", this._uri.Scheme, this._uri.Host, url);
            }

            if (!url.ToLower().StartsWith("http"))
            {
                return string.Format("{0}://{1}/{2}", this._uri.Scheme, this._uri.Host, url);
            }

            if (url.Contains(this._uri.Host))
            {
                return url;
            }

            return null;
        }
    }

    public interface IUrlLevelExtractListener
    {
        void Succeeded(string currentUrl, string htmlContent, int level, List<string> urls);
        void Error(string currentUrl, int level, string exceptionMessage);
    }

    public class UrlLevel
    {
        private int _level;
        private string _rootUrl;

        private UrlExtract _urlExtract = null;
        private IUrlLevelExtractListener _extractListener;

        public UrlLevel(string rootUrl, int level = 0)
        {
            this._urlExtract = new UrlExtract();

            this._rootUrl = rootUrl;
            this._level = level;

            this.Init();
        }

        private void Init()
        {
            if (string.IsNullOrEmpty(this._rootUrl)) throw new ArgumentNullException("rootUrl");
            this._level = this._level < 0 ? 0 : this._level;
        }

        private void ParseUrl(string url, int level)
        {
            if (this._level != 0 && this._level < level) return;

            List<string> urls = this._urlExtract.Execute(url);
            if (string.IsNullOrEmpty(this._urlExtract.GetExceptionMessage()))
            {
                _extractListener.Succeeded(url, this._urlExtract.GetHtmlContent(), level, urls);
            }
            else
            {
                _extractListener.Error(url, level, this._urlExtract.GetExceptionMessage());
            }

            if (urls != null && urls.Count > 0)
            {
                urls.Remove(url);
                foreach (var item in urls)
                {
                    this.ParseUrl(item, level + 1);
                }
            }
        }

        public void Execute(IUrlLevelExtractListener extractListener)
        {
            this._extractListener = extractListener;
            //default level is 0
            this.ParseUrl(this._rootUrl, this._level);
        }

        public void Execute(IUrlLevelExtractListener extractListener, string interruptUrl, int interruptLevel = 0)
        {
            this._extractListener = extractListener;
            this.ParseUrl(interruptUrl, interruptLevel);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ConsoleApplication1.Models.Storage
{
    public class FileOperate
    {
        const string LOCAL_DIRECTORY = "org-content";
        public string _contentUrl;
        private string _htmlContent;
        private bool _contentExists;

        private void SetInfo(string contentUrl, string htmlContent, bool contentExists = false)
        {
            this._contentUrl = contentUrl;
            this._htmlContent = htmlContent;
            this._contentExists = contentExists;
        }

        private string BuildStoreDirectory(string rootUrl)
        {
            string dir = string.Format("{0}/{1}/{2}/", AppDomain.CurrentDomain.BaseDirectory, LOCAL_DIRECTORY, HttpUtility.UrlEncode(rootUrl));
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            return dir;
        }

        private string GetFilePath(string rootUrl, string fileName)
        {
            return string.Format("{0}/{1}", BuildStoreDirectory(rootUrl), fileName);
        }

        public string Save(string rootUrl, string contentUrl, string htmlContent)
        {
            this.SetInfo(null, null, false);

            if (string.IsNullOrEmpty(rootUrl) || string.IsNullOrEmpty(contentUrl) || string.IsNullOrEmpty(htmlContent)) return null;

            try
            {
                string fileName = Guid.NewGuid().ToString();
                File.AppendAllLines(GetFilePath(rootUrl, fileName), new string[] { contentUrl, htmlContent });
                return fileName;
            }
            catch (Exception ex)
            {
                LogHelper.WriteError("Save html content failed : " + ex.Message);
                return null;
            }
        }

        public void Read(string rootUrl, string fileName)
        {
            this.SetInfo(null, null, false);

            if (string.IsNullOrEmpty(rootUrl) || string.IsNullOrEmpty(fileName)) return;

            try
            {
                string[] contents = File.ReadAllLines(GetFilePath(rootUrl, fileName));
                if (contents == null || contents.Length < 2)
                {
                    throw new Exception("null or lenght < 2");
                }

                this.SetInfo(contents[0], contents[1], true);
            }
            catch (FileNotFoundException)
            {
                this.SetInfo(null, null, false);
            }
            catch (Exception ex)
            {
                LogHelper.WriteError("Read html content failed : " + ex.Message);
            }
        }

        public string GetHtml()
        {
            return this._htmlContent;
        }

        public string GetUrl()
        {
            return this._contentUrl;
        }

        public bool IsExists()
        {
            return this._contentExists;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.Models.Coll
{
    public class Common
    {
        public static string GetHtmlContent(string url, Encoding encoding = null)
        {
            if (string.IsNullOrEmpty(url)) return null;
            if (encoding == null) encoding = Encoding.Default;

            string html = null;
            Stream myStream = null;
            WebClient webClient = new WebClient();

            try
            {
                myStream = webClient.OpenRead(url);
                StreamReader sr = new StreamReader(myStream, encoding);
                html = sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (myStream != null) myStream.Close();
            }
            return html;
        }
    }
}

using ConsoleApplication1.Models.Coll;
using ConsoleApplication1.Models.Rule;
using ConsoleApplication1.Models.Sql;
using ConsoleApplication1.Models.Storage;
using ConsoleApplication1.Models.Storage.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.Models.Exec
{
    public class UnravelEntry
    {
        private List<RuleConfig> _configs;

        public UnravelEntry(List<RuleConfig> configs)
        {
            this._configs = configs;
        }

        public void Run()
        {
            Console.WriteLine("Unravel process start run...");
            if (_configs == null || _configs.Count <= 0)
            {
                Console.WriteLine("No rule config found , Unravel process exit.");
                return;
            }
            Console.WriteLine("Unravel process load {0} rule config.", _configs.Count);

            TaskFactory taskFactory = new TaskFactory();
            List<Task> tasks = new List<Task>();
            foreach (RuleConfig item in _configs)
            {
                if (item == null || !item.IsEligible())
                    throw new Exception(string.Format("Check '{0}' rule config items, Url, Key, and Rule for possible null values.", item));

                tasks.Add(taskFactory.StartNew(() =>
                {
                    ArticleHandler ah = new ArticleHandler(item);
                    ah.Process();
                }));
            }

            Console.WriteLine("Unravel process {0} tasks running ...", tasks.Count);
            Task.WaitAll(tasks.ToArray());

            Console.WriteLine("End of run,Unravel process exit.");
        }
    }

    public class ArticleHandler
    {
        private RuleConfig _ruleConfig;
        private FileOperate _fileOperate;
        private ArticleBuilder _articleBuilder;
        private IArticleRule _currentRule;
        private SqlOperate _sqlOperate;

        public ArticleHandler(RuleConfig ruleConfig)
        {
            this._ruleConfig = ruleConfig;
            this._fileOperate = new FileOperate();
            this._articleBuilder = new ArticleBuilder();
            this._sqlOperate = new SqlOperate();

            this._currentRule = new JsonArticleRule(ruleConfig.Rules);
        }

        public void Process()
        {
            UrlStoreModel firstUrl = UrlOperate.GetFirstUrl(this._ruleConfig.Url);
            if (firstUrl == null) return;
            Console.WriteLine("Current url {0}.", firstUrl.Url);

            ArticleModel model = null;
            try
            {
                string htmlContent = this.GetLocalContent(firstUrl);
                if (string.IsNullOrEmpty(htmlContent))
                {
                    Console.WriteLine("Load html content from url.");
                    model = this._articleBuilder.BuildModel(firstUrl.Url, this._currentRule);
                }
                else
                {
                    Console.WriteLine("Load html content from local.");
                    model = this._articleBuilder.BuildModel(firstUrl.Url, htmlContent, this._currentRule);
                }
                Console.WriteLine("Current article title : {0}.", model != null ? model.Title : "");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Article build failed : " + ex.Message);
                Console.ResetColor();

                LogHelper.WriteError("Url store info : " + JsonConvert.SerializeObject(firstUrl) + "/r/nArticle build failed : " + ex.Message);
            }

            //db
            int articleId = this._sqlOperate.AddArticleModel(model);
            Console.WriteLine("Insert article {0} to db.", articleId);

            UrlOperate.MoveUrl(this._ruleConfig.Url, firstUrl);
            Console.WriteLine("Move url {0}.", firstUrl.Url);

            this.Process();
        }

        private string GetLocalContent(UrlStoreModel firstUrl)
        {
            if (firstUrl == null) return null;

            string htmlContent = null;
            if (firstUrl.IsLocalization())
            {
                _fileOperate.Read(this._ruleConfig.Url, firstUrl.localFileName);
                if (_fileOperate.IsExists())
                {
                    htmlContent = _fileOperate.GetHtml();
                }
            }

            return htmlContent;
        }
    }
}

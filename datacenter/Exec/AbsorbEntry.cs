using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using ConsoleApplication1.Models.Rule;
using ConsoleApplication1.Models.Coll;
using ConsoleApplication1.Models.Storage;

namespace ConsoleApplication1.Models.Exec
{
    public class AbsorbEntry
    {
        private List<RuleConfig> _configs;

        public AbsorbEntry(List<RuleConfig> configs)
        {
            this._configs = configs;
        }

        public void Run()
        {
            Console.WriteLine("Absorb process start run...");
            if (_configs == null || _configs.Count <= 0)
            {
                Console.WriteLine("No rule config found , Absorb process exit.");
                return;
            }
            Console.WriteLine("Absorb process load {0} rule config.", _configs.Count);

            TaskFactory taskFactory = new TaskFactory();
            List<Task> tasks = new List<Task>();
            foreach (var item in _configs)
            {
                tasks.Add(taskFactory.StartNew(() =>
                {
                    UrlLevel ul = new UrlLevel(item.Url);
                    ul.Execute(new UrlLevelExtractListener(item.Url));
                }));
            }

            Console.WriteLine("Absorb process {0} tasks running ...", tasks.Count);
            Task.WaitAll(tasks.ToArray());

            Console.WriteLine("End of run,Absorb process exit.");
        }
    }

    public class UrlLevelExtractListener : IUrlLevelExtractListener
    {
        private string _rootUrl;
        private FileOperate _fileOperate;

        public UrlLevelExtractListener(string rootUrl)
        {
            _rootUrl = rootUrl;
            _fileOperate = new FileOperate();
        }

        public void Error(string currentUrl, int level, string exceptionMessage)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(currentUrl + "   " + level + "   " + exceptionMessage);
            Console.ResetColor();

            LogHelper.WriteError(currentUrl + ";" + level + ";" + exceptionMessage);
        }

        public void Succeeded(string currentUrl, string htmlContent, int level, List<string> urls)
        {
            Task task1 = new Task(() =>
            {
                List<string> existUrls = UrlOperate.GetExistUrls(_rootUrl, urls);
                if (existUrls != null && existUrls.Count > 0)
                    urls.RemoveAll(u => existUrls.Contains(u));

                UrlOperate.AddUrls(_rootUrl, urls, level);
            });

            Task task2 = new Task(() =>
            {
                if (UrlOperate.IsLocalization(_rootUrl, currentUrl)) return;

                string fileName = _fileOperate.Save(_rootUrl, currentUrl, htmlContent);
                UrlOperate.UpdateLocalization(_rootUrl, currentUrl, fileName);
            });

            task1.Start();
            task2.Start();
            Task.WaitAll(task1, task2);

            Console.Write(currentUrl + "   " + level + "   " + (urls != null ? urls.Count : 0) + "\r\n");
        }
    }
}
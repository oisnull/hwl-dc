using ConsoleApplication1.Models.Storage.Model;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ConsoleApplication1.Models.Storage
{
    public class UrlOperate
    {
        private const int db = 0;

        private static string GetBeforeUrlKey(string rootUrl)
        {
            return string.Format("{0}:0", HttpUtility.UrlEncode(rootUrl));
        }

        private static string GetAfterUrlKey(string rootUrl)
        {
            return string.Format("{0}:1", HttpUtility.UrlEncode(rootUrl));
        }

        private static string GetHashValue(int level, string localFileName = null)
        {
            if (string.IsNullOrEmpty(localFileName))
            {
                return level.ToString();
            }

            return string.Format("{0},{1}", level, localFileName);
        }

        private static Tuple<int, string> ParseHashValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return new Tuple<int, string>(0, null);

            string[] values = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            int level = 0;
            string localFileName = null;

            switch (values.Length)
            {
                case 1:
                    int.TryParse(values[0], out level);
                    break;
                case 2:
                    int.TryParse(values[0], out level);
                    localFileName = values[1];
                    break;
                default:
                    break;
            }
            return new Tuple<int, string>(level, localFileName);
        }

        public static void AddUrls(string rootUrl, List<string> urls, int level)
        {
            if (string.IsNullOrEmpty(rootUrl) || urls == null || urls.Count <= 0) return;

            RedisUtil.Exec(c =>
            {
                HashEntry[] values = urls.ConvertAll(u => new HashEntry(u, GetHashValue(level))).ToArray();
                c.HashSet(GetBeforeUrlKey(rootUrl), values);
            }, db);
        }

        public static void UpdateLocalization(string rootUrl, string currentUrl, string localFileName)
        {
            if (string.IsNullOrEmpty(rootUrl) || string.IsNullOrEmpty(currentUrl) || string.IsNullOrEmpty(localFileName)) return;

            RedisUtil.Exec(c =>
            {
                RedisValue item = c.HashGet(GetBeforeUrlKey(rootUrl), currentUrl);
                if (!item.HasValue) return;

                Tuple<int, string> val = ParseHashValue(item.ToString());
                if (val.Item2 == localFileName) return;

                c.HashSet(GetBeforeUrlKey(rootUrl), currentUrl, GetHashValue(val.Item1, localFileName));
            }, db);
        }

        public static bool IsLocalization(string rootUrl, string currentUrl)
        {
            if (string.IsNullOrEmpty(rootUrl) || string.IsNullOrEmpty(currentUrl)) return false;

            return RedisUtil.Exec(c =>
            {
                RedisValue item = c.HashGet(GetBeforeUrlKey(rootUrl), currentUrl);
                if (!item.HasValue) return false;

                Tuple<int, string> val = ParseHashValue(item.ToString());
                return !string.IsNullOrEmpty(val.Item2);
            }, db);
        }

        public static List<string> GetExistUrls(string rootUrl, List<string> urls)
        {
            if (string.IsNullOrEmpty(rootUrl) || urls == null || urls.Count <= 0) return null;

            List<string> res = new List<string>();
            RedisUtil.Exec(c =>
            {
                Task<bool>[] tasks_0 = new Task<bool>[urls.Count];
                Task<bool>[] tasks_1 = new Task<bool>[urls.Count];
                for (int i = 0; i < urls.Count; i++)
                {
                    tasks_0[i] = c.HashExistsAsync(GetBeforeUrlKey(rootUrl), urls[i]);
                    tasks_1[i] = c.HashExistsAsync(GetAfterUrlKey(rootUrl), urls[i]);
                }
                c.WaitAll(tasks_0);
                c.WaitAll(tasks_1);

                for (int i = 0; i < tasks_0.Length; i++)
                {
                    if (tasks_0[i].Result)
                    {
                        res.Add(urls[i]);
                    }
                }

                for (int i = 0; i < tasks_1.Length; i++)
                {
                    if (tasks_1[i].Result)
                    {
                        res.Add(urls[i]);
                    }
                }
            }, db);
            return res;
        }

        public static UrlStoreModel GetFirstUrl(string rootUrl)
        {
            return GetUrlByIndex(rootUrl, false);
        }

        public static bool MoveUrl(string rootUrl, UrlStoreModel urlModel)
        {
            if (string.IsNullOrEmpty(rootUrl) || urlModel == null) return false;

            bool succ = false;
            RedisUtil.Exec(c =>
            {
                if (c.HashDelete(GetBeforeUrlKey(rootUrl), urlModel.Url))
                {
                    succ = c.HashSet(GetAfterUrlKey(rootUrl), urlModel.Url, GetHashValue(urlModel.Level, urlModel.localFileName));
                }
            }, db);

            return succ;
        }

        //public static UrlStoreModel GetFirstAndRemoveUrl(string rootUrl)
        //{
        //    return GetUrlByIndex(rootUrl, false, true);
        //}

        public static UrlStoreModel GetLastUrl(string rootUrl)
        {
            return GetUrlByIndex(rootUrl, true);
        }

        private static UrlStoreModel GetUrlByIndex(string rootUrl, bool isLast)
        {
            if (string.IsNullOrEmpty(rootUrl)) return null;

            return RedisUtil.Exec(c =>
            {
                int lastIndex = isLast ? (int)c.HashLength(GetBeforeUrlKey(rootUrl)) - 1 : 0;
                if (lastIndex < 0) return null;

                HashEntry res = c.HashScan(GetBeforeUrlKey(rootUrl), "", 1, 1, lastIndex).FirstOrDefault();

                Tuple<int, string> val = ParseHashValue(res.Value.ToString());
                return new UrlStoreModel()
                {
                    Url = res.Name.ToString(),
                    Level = val.Item1,
                    localFileName = val.Item2
                };
            }, db);
        }

        public static List<UrlStoreModel> GetHaveContentUrls(string rootUrl)
        {
            if (string.IsNullOrEmpty(rootUrl)) return null;

            List<UrlStoreModel> urls = new List<UrlStoreModel>();
            RedisUtil.Exec(c =>
            {
                HashEntry[] items = c.HashGetAll(GetBeforeUrlKey(rootUrl));
                UrlStoreModel model = null;
                Tuple<int, string> value = null;
                foreach (HashEntry item in items)
                {
                    value = ParseHashValue(item.Value.ToString());
                    model = new UrlStoreModel()
                    {
                        Url = item.Name.ToString(),
                        Level = value.Item1,
                        localFileName = value.Item2
                    };

                    if (model.IsLocalization())
                        urls.Add(model);
                }
            }, db);

            return urls;
        }
    }
}

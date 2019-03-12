using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ConsoleApplication1.Models.Rule
{
    public class RuleConfig
    {
        public string Url { get; set; }
        public List<ExtractRuleConfig> Rules { get; set; }

        public bool IsEligible()
        {
            return !string.IsNullOrEmpty(this.Url)
                && this.Rules != null
                && this.Rules.Count > 0
                && !this.Rules.Exists(r => string.IsNullOrEmpty(r.Key) || r.Rule == null);
        }
    }

    public class ExtractRuleConfig
    {
        public string Key { get; set; }
        public bool IsList { get; set; }
        public RuleModel Rule { get; set; }
    }

    public class RuleConfigBuilder
    {

        public static string GetConfigString(string configPath)
        {
            if (string.IsNullOrEmpty(configPath)) throw new Exception("rule config path is empty");

            using (FileStream fs = new FileStream(configPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader sr = new StreamReader(fs, Encoding.GetEncoding("UTF-8")))
                {
                    return sr.ReadToEnd().ToString();
                }
            }
        }

        public static List<RuleConfig> GetConfigs(string configPath)
        {
            string jsonString = GetConfigString(configPath);
            if (string.IsNullOrEmpty(jsonString)) throw new Exception("rule config content is empty");

            return JsonConvert.DeserializeObject<List<RuleConfig>>(jsonString);
        }
    }
}
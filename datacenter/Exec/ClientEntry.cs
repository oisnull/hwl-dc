using ConsoleApplication1.Models.Rule;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.Models.Exec
{
    public class ClientEntry
    {
        const string CONFIG_FILE_NAME = "/models/rule/rule.json";
        private List<RuleConfig> _configs;

        //absorb
        //unravel
        //refine
        private AbsorbEntry absorbEntry;
        private UnravelEntry unravelEntry;

        public ClientEntry()
        {
            this.LoadConfig();
            this.absorbEntry = new AbsorbEntry(this._configs);
            this.unravelEntry = new UnravelEntry(this._configs);
        }

        private void LoadConfig()
        {
            string configPath = AppDomain.CurrentDomain.BaseDirectory + CONFIG_FILE_NAME;
            if (File.Exists(configPath))
            {
                _configs = RuleConfigBuilder.GetConfigs(configPath);
                return;
            }

            configPath = Path.GetFullPath("../..") + CONFIG_FILE_NAME;
            if (File.Exists(configPath))
            {
                _configs = RuleConfigBuilder.GetConfigs(configPath);
                return;
            }

            throw new Exception("No rule config file found.");
        }

        public void Run()
        {
            Console.WriteLine("Client start run...");
            if (_configs == null || _configs.Count <= 0)
            {
                Console.WriteLine("No rule config found , client exit.");
                return;
            }

            //this.absorbEntry.Run();
            this.unravelEntry.Run();

            Console.WriteLine("Client end.");
        }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.Models.Storage.Model
{
    public class UrlStoreModel
    {
        public string Url { get; set; }
        public int Level { get; set; }
        public string localFileName { get; set; }

        public bool IsLocalization()
        {
            return !string.IsNullOrEmpty(this.localFileName);
        }
    }
}

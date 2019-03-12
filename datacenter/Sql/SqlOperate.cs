using ConsoleApplication1.Models.Coll;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.Models.Sql
{
    public class SqlOperate
    {
        private SqlDBHelper dbContext = null;

        public SqlOperate()
        {
            dbContext = new SqlDBHelper("Data Source=172.18.9.136;Initial Catalog=MyData;Integrated Security=False;User ID=sa;Password=Zeng.2018;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        }

        public int AddArticleModel(ArticleModel model)
        {
            if (model == null) return 0;

            string sql = string.Format(@"INSERT INTO [dbo].[Article](OriginUrl,Title,Keys,Author,Summary,ImageUrls,Content,PublishDate,CollectionDate) 
                VALUES(@OriginUrl,@Title,@Keys,@Author,@Summary,@ImageUrls,@Content,@PublishDate,@CollectionDate)SELECT @@IDENTITY");

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@OriginUrl", CutString(model.OriginUrl, 500)));
            parameters.Add(new SqlParameter("@Title", CutString(model.Title, 500)));
            parameters.Add(new SqlParameter("@Keys", CutString(model.Keys, 500)));
            parameters.Add(new SqlParameter("@Author", CutString(model.Author, 50)));
            parameters.Add(new SqlParameter("@Summary", CutString(model.Summary, 1000)));
            parameters.Add(new SqlParameter("@ImageUrls", CutString(this.GetImageUrls(model.ImageUrls), 0)));
            parameters.Add(new SqlParameter("@Content", CutString(model.Content, 0)));
            parameters.Add(new SqlParameter("@PublishDate", CutString(model.PublishDate, 50)));
            parameters.Add(new SqlParameter("@CollectionDate", DateTime.Now));

            int articleId;
            int.TryParse(dbContext.ExecuteScalar(sql, System.Data.CommandType.Text, parameters.ToArray()).ToString(), out articleId);

            return articleId;
        }

        private string GetImageUrls(List<string> imgs)
        {
            if (imgs == null || imgs.Count <= 0) return "";
            return string.Join(",", imgs);
        }
        //People burn a poster depicting India's flag against what they call airspace violation by the Indian military aircrafts, in a protest in Peshawar, Pakistan February 26, 2019. ©REUTERS / Fayaz Aziz 
        private string CutString(string str, int cutLength)
        {
            if (string.IsNullOrEmpty(str)) return "";

            if (cutLength <= 0 || str.Length <= cutLength) return str;

            return str.Substring(0, cutLength);
        }
    }
}

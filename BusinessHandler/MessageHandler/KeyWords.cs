using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BusinessHandler.MessageHandler
{
    public interface IKeyWord
    {
        List<KeyWordModel> GetKeyWordList();

        bool AddKeyWord(string keyWord, out string msg);
    }

    public class KeyWordRepository : IKeyWord
    {
        public List<KeyWordModel> GetKeyWordList()
        {
            List<KeyWordModel> list = new List<KeyWordModel>();

            var fileName = HttpContext.Current.Server.MapPath("~/File/KeyWord.json");
            var json = File.ReadAllText(fileName);
            var jobj = JArray.Parse(json);
            list = jobj.Select(x => new KeyWordModel { KeyWord = x["KeyWord"].ToString(), AddDate = x["AddDate"].ToString() })
                       .ToList();

            return list;
        }

        public bool AddKeyWord(string keyWord, out string msg)
        {
            msg = "Success";
            var list = GetKeyWordList();
            if(list.Any(x=>x.KeyWord.Equals(keyWord,StringComparison.OrdinalIgnoreCase)))
            {
                msg = "duplicate key word";
                return false;
            }
            else
            {
                list.Add(new KeyWordModel { KeyWord = keyWord, AddDate = DateTime.Now.ToString() });
            }
            string json = JsonConvert.SerializeObject(list.ToArray());

            var fileName = HttpContext.Current.Server.MapPath("~/File/KeyWord.json");
            System.IO.File.WriteAllText(fileName, json);
            return true;
        }
    }
    public class KeyWordModel
    {
        public string KeyWord { get; set; }
        public string AddDate { get; set; }
    }
}

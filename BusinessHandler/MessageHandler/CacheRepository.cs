using BusinessHandler.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BusinessHandler.MessageHandler
{
    public interface ICacheRepository
    {
        void Add(string key, List<DocQueryResultModel> docQueryList);
        void Clear(string key);
        bool Exists(string key);
        List<DocQueryResultModel> Get (string key);

    }

    public class AspNetCacheRepository : ICacheRepository
    {
        public void Add(string key, List<DocQueryResultModel> docQueryList)
        {
            HttpContext.Current.Cache.Insert(
             key,
             docQueryList,
             null,
             DateTime.Now.AddMinutes(20),
             System.Web.Caching.Cache.NoSlidingExpiration);
        }
        public void Clear(string key)
        {
            HttpContext.Current.Cache.Remove(key);
        }
        public bool Exists(string key)
        {
            return HttpContext.Current.Cache[key] != null;
        }
        public List<DocQueryResultModel> Get(string key)
        {
            try
            {
                return (List<DocQueryResultModel>)HttpContext.Current.Cache[key];
            }
            catch
            {
                return null;
            }
        }
    }

}


using BusinessHandler.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BusinessHandler.MessageHandler
{
   public class DocQueryFactory
    {
        IDocQueryRepository docQueryRepository;
        ICacheRepository cacheRepository;
        public const string docQueryCacheKey = "docQueryCacheKey";
        public  DocQueryFactory()
        {
            //docQueryRepository = new DocQueryCSVRepository();// DependencyResolver.Current.GetService<IDocQueryRepository>();
            //cacheRepository = new AspNetCacheRepository();// DependencyResolver.Current.GetService<ICacheRepository>();

            docQueryRepository =  DependencyResolver.Current.GetService<IDocQueryRepository>();
            cacheRepository =  DependencyResolver.Current.GetService<ICacheRepository>();

        }
        public List<DocQueryResultModel> GetDocQueryResult(DocQueryMessage message)
        {
            var list = new List<DocQueryResultModel>();
            list = docQueryRepository.GetDocQueryResult(message);
            return list;
        }
        public void UpdateQuery(DocQueryResultModel message)
        {
            docQueryRepository.UpdateQuery(message);
            if (cacheRepository.Exists(docQueryCacheKey))
            {
                cacheRepository.Clear(docQueryCacheKey);
            }
        }

    }
}

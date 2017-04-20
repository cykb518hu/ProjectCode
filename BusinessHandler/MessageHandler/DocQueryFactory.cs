
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
        public  DocQueryFactory()
        {

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
            if (cacheRepository.Exists(GlobalKeyString.docQueryCacheKey))
            {
                cacheRepository.Clear(GlobalKeyString.docQueryCacheKey);
            }
        }


    }
}

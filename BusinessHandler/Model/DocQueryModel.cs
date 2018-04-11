using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessHandler.Model
{
    public class DocQueryMessage
    {
        public int limit { get; set; }
        public int offset { get; set; }
        public string CityName { get; set; }
        public string CountyName { get; set; }
        public string MeetingDate { get; set; }
        public string KeyWord { get; set; }
        public string DeployDate { get; set; }
        public string sortName { get; set; }
        public string sortOrder { get; set; }
        public string StartMeetingDate { get; set; }
        public string EndMeetingDate { get; set; }

        public string IsViewed { get; set; }
        public string Important { get; set; }

        public string Note { get; set; }
        public string State { get; set; }
        public string ObjectIds { get; set; }
    }


    //use for update, can optimize later
    public class DocQueryResultModel
    {
        public string QueryGuid { get; set; }
        public string DocId { get; set; }
        public string CityName { get; set; }
        
        public string CityDeployDate { get; set; }
        public string CityNameDispaly { get; set; }

        public string DocUrl { get; set; }
        public string DocType { get; set; }
        public string MeetingTitle { get; set; }
        public DateTime MeetingDate { get; set; }
        public string MeetingDateDisplay { get; set; }
        public string MeetingLocation { get; set; }
        public string KeyWord { get; set; }
        public string Content { get; set; }
        public string Operation { get; set; }
        public string DocFilePath { get; set; }
        public string QueryFilePath { get; set; }
        public string Comment { get; set; }
        public string PageNumber { get; set; }
        public string IsViewed { get; set; }

        public string ScrapeDate { get; set; }
        public string Important { get; set; }
    }


  
   
}

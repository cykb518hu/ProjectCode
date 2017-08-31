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

        public string IsViewed { get; set; }
        public string Important { get; set; }
    }

    public class DocQueryParentModel
    {
        public string IsViewed { get; set; }
        public string DocUrl { get; set; }
        public string DocId { get; set;}
        public int Number { get; set; }
        public string DocType { get; set; }
        public string KeyWordString { get; set; }
        public string CityDeployDate { get; set; }

     

        public string DocFilePath { get; set; }
        public List<DocQueryResultModel> DocQuerySubList { get; set; }

        public string ScrapeDate { get; set; }

        public string CityNameDispaly { get; set; }
        public string MeetingDateDisplay { get; set; }

        public string Removed { get; set; }

        public string ImportantDisplay { get; set; }

        public string MunicipalityDispaly { get; set; }
        public string MinicipalityOperation { get; set; }
        public string COMMENT { get; set; }

        public string LongName { get; set; }
    }
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

    public class DocData
    {
        public string CityName { get; set; }
        public string DocId { get; set; }
        public string LocalPath { get; set; }
        public string DocType { get; set; }
        public string DocUrl { get; set; }
        public string CanBeRead { get; set; }
        public string DocFilePath { get; set; }

        public string IsViewed { get; set; }

        public string Important { get; set; }
    }
    public class QueryData
    {
        public string QueryGuid { get; set; }
        public string CityName { get; set; }
        public string DocId { get; set; }
        public string MeetingTitle { get; set; }
        public DateTime MeetingDate { get; set; }
        public string MeetingDateDisplay { get; set; }
        public string MeetingLocation { get; set; }
        public string KeyWord { get; set; }
        public string Content { get; set; }
        public string QueryFilePath { get; set; }
        public string Comment { get; set; }

        public string PageNumber { get; set; }
        public string ScrapeDate { get; set; }
    }

    public class QueryXmlData
    {
        public string CityName { get; set; }
        public string DocId { get; set; }
        public string MeetingTitle { get; set; }
        public string MeetingDateDisplay { get; set; }
        public string MeetingLocation { get; set; }
        public DateTime MeetingDate { get; set; }
        public string SearchDate { get; set; }
        public List<QueryEntryXmlData> Entries { get; set; }

    }
    public class QueryEntryXmlData
    {
        public string KeyWord { get; set; }
     
        public string PageNumber { get; set; }

        public List<QueryContentXmlData> ContentList { get; set; }
    }
    public class QueryContentXmlData
    {
        public string Comment { get; set; }
        public string Content { get; set; }

        public string QueryGuid { get; set; }
    }

    #region map model

    public class MapMunicipality
    {
        public string MunicipalityName { get; set; }

        public string CountyName { get; set; }

        public string DeployDate { get; set; }

        public int DocAmount { get; set; }
        public int KeyWordAmount { get; set; }
        public List<MapMunicipalityComment> CommentList { get; set; }
    }
    public class MapMunicipalityColor
    {
        public string MunicipalityName { get; set; }
        public string Color { get; set; }
    }
    public class MapMunicipalityComment
    {
        public string MeetingDate { get; set; }
        public string Comment { get; set; }
        public string AddDate { get; set; }
    }
    #endregion

}

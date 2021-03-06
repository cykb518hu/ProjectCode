﻿using System.Collections.Generic;

namespace BusinessHandler.Model
{

    public class MapFilterModel
    {
        public string CountyName { get; set; }
        public string MunicipalityName { get; set; }
        public string DeployDate { get; set; }
        public string CityGuid { get; set; }
    }

    public class MapMunicipalityColor
    {
        public string MunicipalityName { get; set; }
        public int Id { get; set; }
        public string Color { get; set; }
        public string Guid { get; set; }
    }


    public class MapMeeting
    {
        public string DocId { get; set; }

        public string DocType { get; set; }
        public string MeetingDateDisplay { get; set; }

        public string CityDeployDate { get; set; }

        public string ScrapeDate { get; set; }

        public string MinicipalityOperation { get; set; }

        public string MunicipalityDispaly { get; set; }

        public string IsViewed { get; set; }
        public List<MapMeetingKeyWord> DocQuerySubList { get; set; }
        public int ObjectId { get; set; }

    }


    public class MapMeetingKeyWord
    {
        public string DocId { get; set; }
        public string DocContentId { get; set; }

        public int PageNumber { get; set; }
        public string KeyWord { get; set; }
        public string Content { get; set; }
      

       // public string Comment { get; set; }
        public string Operation { get; set; }


    }

    public class MapMeetingCity
    {
        public string CityName { get; set; }
        public string CityLongName { get; set; }
        public string Color { get; set; }
        public List<MapMeetingNote> MeetingList { get; set; }
    }

    public class MapMeetingNote
    {
        public string DocGuid { get; set; }
        public string CityName { get; set; }
        public string MeetingDate { get; set; }
        public string ScrapeDate { get; set; }
        public string DocType { get; set; }
        public string DeployDate { get; set; }
        public List<MeetingNote> NoteList { get; set; }
        public string Operation { get; set; }
        public string Removed { get; set; }
    }

    public class MeetingNote
    {
        public string Note { get; set; }
        public string NoteEdit { get; set; }
        public string DocGuid { get; set; }
        public string Guid { get; set; }
        public string CreateDate { get; set; }
        public string ModifyDate { get; set; }
        public string Status { get; set; }
        public string Operation { get; set; }

        public string ModifyUser { get; set; }

        public string Tags { get; set; }

        public List<NoteTags> AllTags { get; set; }


    }
    public class NoteTags
    {
        public string tag { get; set; }
    }


    public class MeetingCalendar
    {
        public string CityName { get; set; }
        public string DocType { get; set; }
        public string Note { get; set; }
        public string DocGuid { get; set; }
        public string FutureDate { get; set; }


    }

    public class MeetingTypeTime
    {
        public string MeetingType { get; set; }
        public string LastMeeting { get; set; }
        public string LastScrape { get; set; }
    }



}

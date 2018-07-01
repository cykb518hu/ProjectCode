using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessHandler.Model
{
    public class Dashboard
    {
        public int NumberOfCities { get; set; }

        public List<RecentScrape> RecentScrapes { get; set; }

        public List<RecentMeeting> RecentMeetings { get; set; }

        public List<MeetingLineGraph> MeetingLineGraphData { get; set; }

        public string MeetingLineGraphJsonData
        {
            get { return JsonConvert.SerializeObject(MeetingLineGraphData); }
        }
    }


    public class RecentScrape
    {
        public string CityName { get; set; }
        public string DeployDate { get; set; }
    }

    public class RecentMeeting
    {
        public string CityName { get; set; }
        public string Meeting { get; set; }
        public string MeetingDate { get; set; }
    }

    public class MeetingLineGraph
    {
        [JsonProperty("period")]
        public string Period { get; set; }
        [JsonProperty("meetingDateAmount")]
        public int MeetingDateAmount { get; set; }
        [JsonProperty("scrapeDateAmount")]
        public int ScrapeDateAmount { get; set; }
    }
}

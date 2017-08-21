using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessHandler.Model
{
   public class MapCounty
    {

        
        public string County { get; set; }
        public string ModifyDate { get; set; }
        public bool Important { get; set; }
        public bool Reviewd { get; set; }


        public static List<MapCounty> GetMapCountyList()
        {
            var fileName = @"C:\TestCode\Project\InteractiveMap\County.json";
            var json = File.ReadAllText(fileName);
            var list = JsonConvert.DeserializeObject<List<MapCounty>>(json);
            return list;
        }
    }
}

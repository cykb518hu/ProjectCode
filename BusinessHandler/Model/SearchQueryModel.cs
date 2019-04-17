using System;
using System.Xml.Serialization;

namespace BusinessHandler.Model
{
    [XmlType("SearchQuery")]
    public class SearchQueryModel
    {
        public SearchQueryModel()
        {
            Guid = System.Guid.NewGuid().ToString();
            CreateDate = DateTime.Now;
            ModifyDate = DateTime.Now;
            FrequentlyUsed = 1;
            Disabled = false;
        }
        public string Guid { get; set; }

        public string Title { get; set; }
        public string Content { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime  ModifyDate { get; set; }

        public int FrequentlyUsed { get; set; }

        public bool Disabled { get; set; }
    }
}

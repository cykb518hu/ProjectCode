﻿using System.Collections.Generic;
using System.Xml.Serialization;

namespace BusinessHandler.Model
{


    public class UserAccount
    {
        [XmlElement("Email")]
        public string Email { get; set; }

        [XmlElement("Password")]
        public string Password { get; set; }

        [XmlElement("Cityes")]
        public string Cityes { get; set; }

        [XmlElement("Active")]
        public string Active { get; set; }

        public string RoleType { get; set; }

        public string Operation { get; set; }

        public List<string> CityList { get; set; }

        public string AddDate { get; set; }

        public string CityGuid { get; set; }

        public string Source { get; set; }

    }
}

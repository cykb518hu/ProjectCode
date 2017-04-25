using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    }
}

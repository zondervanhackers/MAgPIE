using System;
using System.Xml.Serialization;

namespace ZondervanLibrary.Harvester.Service.Configuration
{
    [XmlType("Notification")]
    public class NotificationSettings
    {
       public EmailAccount SystemEmailAccount { get; set; }

        [XmlArrayItem("Email")]
        public String[] DeveloperEmails { get; set; }

        [XmlArrayItem("Email")]
        public String[] UserEmails { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Configuration;
using System.IO;

namespace LNF.Logging.Service
{
    [DataContract]
    public abstract class ServiceLogMessage
    {
        [DataMember]
        public Guid MessageID { get; set; }
        [DataMember]
        public DateTime Timestamp { get; set; }
        [DataMember]
        public string From { get; set; }

        public abstract string LogFileName { get; }

        public abstract void UpdateNode(XmlNode node);

        public string LogPath()
        {
            return Path.Combine(ConfigurationManager.AppSettings["LogLocation"], LogFileName);
        }

        protected void AddAttribute(XmlNode node, string name, object value, string format = null)
        {
            XmlAttribute attr;
            attr = node.OwnerDocument.CreateAttribute(name);
            if (string.IsNullOrEmpty(format))
                attr.Value = value.ToString();
            else
                attr.Value = string.Format(format, value);
            node.Attributes.Append(attr);
        }
    }
}

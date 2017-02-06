using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Runtime.Serialization;
using System.Configuration;

namespace LNF.Logging.Service
{
    [DataContract]
    public class PageNotFoundLogMessage : ServiceLogMessage
    {
        [DataMember]
        public string IP { get; set; }
        [DataMember]
        public string Path { get; set; }

        public override string LogFileName
        {
            get { return "404.xml"; }
        }

        public override void UpdateNode(XmlNode node)
        {
            AddAttribute(node, "ip", IP);
            AddAttribute(node, "path", Path);
        }
    }
}

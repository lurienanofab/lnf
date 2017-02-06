using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;
using System.Xml;

namespace LNF.Logging.Service
{
    public class DatabaseLogMessage : ServiceLogMessage
    {
        public string SQL { get; set; }
        public int Count { get; set; }

        public override string LogFileName
        {
            get { return "dblog.xml"; }
        }

        public override void UpdateNode(XmlNode node)
        {
            AddAttribute(node, "sql", SQL);
            AddAttribute(node, "count", Count);
        }
    }
}

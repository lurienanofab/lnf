using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace LNF.Repository.Meter
{
    public class MeterServiceLog : IDataItem
    {
        public virtual int ServiceLogID { get; set; }
        public virtual string LogMessageType { get; set; }
        public virtual string LogMessage { get; set; }
        public virtual DateTime EntryDateTime { get; set; }
        public virtual string EntryData { get; set; }

        public virtual XmlDocument GetXML()
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(EntryData);
            return xdoc;
        }
    }
}

using LNF.DataAccess;
using LNF.Impl.Repository.Data;
using System;
using System.Xml;

namespace LNF.Impl.Repository.Ordering
{
    public class Tracking : IDataItem
    {
        public virtual int TrackingID { get; set; }
        public virtual TrackingCheckpoint TrackingCheckpoint { get; set; }
        public virtual PurchaseOrder PurchaseOrder { get; set; }
        public virtual Client Client { get; set; }
        public virtual string TrackingData { get; set; }
        public virtual DateTime TrackingDateTime { get; set; }

        public virtual XmlDocument GetDataAsXML()
        {
            if (string.IsNullOrEmpty(TrackingData)) return null;
            XmlDocument result = new XmlDocument();
            result.LoadXml(TrackingData);
            return result;
        }
    }
}

using System;
using System.Data;
using System.Xml;

namespace LNF.Ordering
{
    public static class Tracking
    {
        public static void Track(TrackingCheckpoints checkpoint, IPurchaseOrder po, int clientId, XmlDocument trackingData)
        {
            InsertNewTrackingItem(checkpoint, po, clientId, trackingData);
        }

        public static void Track(TrackingCheckpoints checkpoint, IPurchaseOrder po, int clientId, object trackingData)
        {
            XmlDocument xdoc = new XmlDocument();
            XmlNode root = xdoc.CreateElement("data");
            XmlElement add;
            XmlAttribute attr;

            xdoc.AppendChild(root);

            foreach (var x in trackingData.GetType().GetProperties())
            {
                string key = x.Name;
                object obj = x.GetValue(trackingData, null);
                string value = (obj == null) ? string.Empty : obj.ToString();

                add = xdoc.CreateElement("add");
                
                attr = xdoc.CreateAttribute("key");
                attr.Value = key;
                add.Attributes.Append(attr);

                attr = xdoc.CreateAttribute("value");
                attr.Value = value;
                add.Attributes.Append(attr);

                root.AppendChild(add);
            };

            Track(checkpoint, po, clientId, xdoc);
        }

        public static void Track(TrackingCheckpoints checkpoint, IPurchaseOrder po, int clientId, string trackingData)
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(trackingData);
            Track(checkpoint, po, clientId, xdoc);
        }

        public static void Track(TrackingCheckpoints checkpoint, IPurchaseOrder po, int clientId)
        {
            InsertNewTrackingItem(checkpoint, po, clientId, null);
        }

        public static void Track(TrackingCheckpoints checkpoint, int poid, int clientId)
        {
            var po = ServiceProvider.Current.Ordering.PurchaseOrder.GetPurchaseOrder(poid);

            if (po == null)
                throw new ItemNotFoundException("PurchaseOrder", "POID", poid);

            InsertNewTrackingItem(checkpoint, po, clientId, null);
        }

        private static ITracking InsertNewTrackingItem(TrackingCheckpoints checkpoint, IPurchaseOrder po, int clientId, XmlDocument trackingData)
        {
            if (!IsActiveCheckpoint(checkpoint))
                return null;

            string data;

            if (trackingData == null)
                data = null;
            else
                data = trackingData.OuterXml;

            return ServiceProvider.Current.Ordering.Tracking.AddTracking(checkpoint, po.POID, clientId, data);
        }

        public static bool IsActiveCheckpoint(TrackingCheckpoints checkpoint)
        {
            ITrackingCheckpoint tp = Find(checkpoint);
            if (tp == null) return false;
            return tp.Active;
        }

        public static bool Is(ITrackingCheckpoint item, TrackingCheckpoints checkpoint)
        {
            return item.TrackingCheckpointID == Convert.ToInt32(checkpoint);
        }

        public static ITrackingCheckpoint Find(TrackingCheckpoints checkpoint)
        {
            return ServiceProvider.Current.Ordering.Tracking.GetCheckpoint(checkpoint);
        }

        public static DataTable SelectTrackingData(int poid)
        {
            var dt = new DataTable();
            dt.Columns.Add("TrackingID", typeof(int));
            dt.Columns.Add("POID", typeof(int));
            dt.Columns.Add("ClientID", typeof(int));
            dt.Columns.Add("TrackingDateTime", typeof(DateTime));
            dt.Columns.Add("CheckpointID", typeof(int));
            dt.Columns.Add("CheckpointText", typeof(string));
            dt.Columns.Add("TrackingData", typeof(string));

            var trackingItems = ServiceProvider.Current.Ordering.Tracking.GetTracking(poid);
            var checkpointItems = ServiceProvider.Current.Ordering.Tracking.GetCheckpoints();

            foreach (var item in trackingItems)
            {
                var ndr = dt.NewRow();
                ndr.SetField("TrackingID", item.TrackingID);
                ndr.SetField("POID", item.POID);
                ndr.SetField("ClientID", item.ClientID);
                ndr.SetField("TrackingDateTime", item.TrackingDateTime);
                ndr.SetField("CheckpointID", item.TrackingCheckpointID);
                ndr.SetField("CheckpointText", GetCheckpointText(item));
                ndr.SetField("TrackingData", item.TrackingData);
                dt.Rows.Add(ndr);
            }

            return dt;
        }

        public static string GetCheckpointText(ITracking item)
        {
            return string.Format("{0} by {1}, {2}", item.CheckpointName, item.LName, item.FName);
        }
    }
}

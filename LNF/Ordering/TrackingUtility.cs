using System.Linq;
using System.Data;
using LNF.Data;
using LNF.Repository;
using LNF.Repository.Ordering;
using System;
using System.Xml;

namespace LNF.Ordering
{
    public static class TrackingUtility
    {
        public static void Track(TrackingCheckpoints checkpoint, PurchaseOrder po, int clientId, XmlDocument trackingData)
        {
            InsertNewTrackingItem(checkpoint, po, clientId, trackingData);
        }

        public static void Track(TrackingCheckpoints checkpoint, PurchaseOrder po, int clientId, object trackingData)
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

        public static void Track(TrackingCheckpoints checkpoint, PurchaseOrder po, int clientId, string trackingData)
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(trackingData);
            Track(checkpoint, po, clientId, xdoc);
        }

        public static void Track(TrackingCheckpoints checkpoint, PurchaseOrder po, int clientId)
        {
            InsertNewTrackingItem(checkpoint, po, clientId, null);
        }

        public static void Track(TrackingCheckpoints checkpoint, int poid, int clientId)
        {
            var po = DA.Current.Single<PurchaseOrder>(poid);

            if (po == null)
                throw new ItemNotFoundException<PurchaseOrder>(x => x.POID, poid);

            InsertNewTrackingItem(checkpoint, po, clientId, null);
        }

        private static void InsertNewTrackingItem(TrackingCheckpoints checkpoint, PurchaseOrder po, int clientId, XmlDocument trackingData)
        {
            if (!IsActiveCheckpoint(checkpoint)) return;

            string data;

            if (trackingData == null)
                data = null;
            else
                data = trackingData.OuterXml;

            Tracking track = new Tracking
            {
                TrackingCheckpoint = Find(checkpoint),
                PurchaseOrder = po,
                Client = ClientUtility.Find(clientId),
                TrackingData = data,
                TrackingDateTime = DateTime.Now
            };

            DA.Current.Insert(track);
        }

        public static bool IsActiveCheckpoint(TrackingCheckpoints checkpoint)
        {
            TrackingCheckpoint tp = Find(checkpoint);
            if (tp == null) return false;
            return tp.Active;
        }

        public static bool Is(TrackingCheckpoint item, TrackingCheckpoints checkpoint)
        {
            return item.TrackingCheckpointID == Convert.ToInt32(checkpoint);
        }

        public static TrackingCheckpoint Find(TrackingCheckpoints checkpoint)
        {
            return DA.Current.Single<TrackingCheckpoint>(Convert.ToInt32(checkpoint));
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

            var trackingItems = DA.Current.Query<Tracking>().Where(x => x.PurchaseOrder.POID == poid).ToList();
            var checkpointItems = DA.Current.Query<TrackingCheckpoint>().ToList();

            foreach (var item in trackingItems)
            {
                var ndr = dt.NewRow();
                ndr.SetField("TrackingID", item.TrackingID);
                ndr.SetField("POID", item.PurchaseOrder.POID);
                ndr.SetField("ClientID", item.Client.ClientID);
                ndr.SetField("TrackingDateTime", item.TrackingDateTime);
                ndr.SetField("CheckpointID", item.TrackingCheckpoint.TrackingCheckpointID);
                ndr.SetField("CheckpointText", GetCheckpointText(item));
                ndr.SetField("TrackingData", item.TrackingData);
                dt.Rows.Add(ndr);
            }

            return dt;
        }

        public static string GetCheckpointText(Tracking item)
        {
            return string.Format("{0} by {1}, {2}", item.TrackingCheckpoint.CheckpointName, item.Client.LName, item.Client.FName);
        }
    }
}

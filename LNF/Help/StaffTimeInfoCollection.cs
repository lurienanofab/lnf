using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace LNF.Help
{
    public class StaffTimeInfoCollection : IEnumerable<KeyValuePair<DayOfWeek, StaffTimeInfo>>
    {
        public StaffTimeInfoCollection()
        {
            Init();
        }

        public StaffTimeInfoCollection(string xml)
        {
            Init();
            XmlDocument xdoc = new XmlDocument();
            if (!string.IsNullOrEmpty(xml)) xdoc.LoadXml(xml);
            Load(DayOfWeek.Monday, xdoc);
            Load(DayOfWeek.Tuesday, xdoc);
            Load(DayOfWeek.Wednesday, xdoc);
            Load(DayOfWeek.Thursday, xdoc);
            Load(DayOfWeek.Friday, xdoc);
            Load(DayOfWeek.Saturday, xdoc);
            Load(DayOfWeek.Sunday, xdoc);
        }

        private Dictionary<DayOfWeek, StaffTimeInfo> _Items;

        public StaffTimeInfo this[DayOfWeek index]
        {
            get { return _Items[index]; }
            set { _Items[index] = value; }
        }

        public int Count
        {
            get { return _Items.Count; }
        }

        private void Init()
        {
            _Items = new Dictionary<DayOfWeek, StaffTimeInfo>();
            AddItem(DayOfWeek.Sunday);
            AddItem(DayOfWeek.Monday);
            AddItem(DayOfWeek.Tuesday);
            AddItem(DayOfWeek.Wednesday);
            AddItem(DayOfWeek.Thursday);
            AddItem(DayOfWeek.Friday);
            AddItem(DayOfWeek.Saturday);
        }

        private void AddItem(DayOfWeek dow)
        {
            _Items.Add(dow, new StaffTimeInfo(dow));
        }

        public void Load(DayOfWeek dow, XmlDocument xdoc)
        {
            string d = Enum.GetName(typeof(DayOfWeek), dow).ToLower();
            XmlNode node = xdoc.SelectSingleNode("/staff_time/" + d);
            StaffTimeInfo info = _Items[dow];
            if (node != null)
            {
                info.Checked = node.Attributes["checked"].Value == "1";
                XmlNode am = node.SelectSingleNode("am");
                info.AM.Start = new StaffTimeValue(am.Attributes["start"].Value);
                info.AM.End = new StaffTimeValue(am.Attributes["end"].Value);
                XmlNode pm = node.SelectSingleNode("pm");
                info.PM.Start = new StaffTimeValue(pm.Attributes["start"].Value);
                info.PM.End = new StaffTimeValue(pm.Attributes["end"].Value);
            }
            else
            {
                info.Checked = false;
                info.AM.Start = new StaffTimeValue();
                info.AM.End = new StaffTimeValue();
                info.PM.Start = new StaffTimeValue();
                info.PM.End = new StaffTimeValue();
            }
        }

        public void Load(DayOfWeek dow, bool Checked, string StartAM, string EndAM, string StartPM, string EndPM)
        {
            _Items[dow].Checked = Checked;
            _Items[dow].AM.Start = new StaffTimeValue(StartAM);
            _Items[dow].AM.End = new StaffTimeValue(EndAM);
            _Items[dow].PM.Start = new StaffTimeValue(StartPM);
            _Items[dow].PM.End = new StaffTimeValue(EndPM);
        }

        public XmlDocument ToXML()
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml("<staff_time></staff_time>");
            AddNode(DayOfWeek.Monday, xdoc);
            AddNode(DayOfWeek.Tuesday, xdoc);
            AddNode(DayOfWeek.Wednesday, xdoc);
            AddNode(DayOfWeek.Thursday, xdoc);
            AddNode(DayOfWeek.Friday, xdoc);
            AddNode(DayOfWeek.Saturday, xdoc);
            AddNode(DayOfWeek.Sunday, xdoc);
            return xdoc;
        }

        private XmlNode AddNode(DayOfWeek dow, XmlDocument xdoc)
        {
            XmlAttribute attr;

            StaffTimeInfo info = _Items[dow];

            XmlNode root = xdoc.SelectSingleNode("/staff_time");

            bool is_checked = info.Checked;
            if (is_checked)
            {
                if (string.IsNullOrEmpty(info.AM.Start.ToString()) && string.IsNullOrEmpty(info.AM.End.ToString()) && string.IsNullOrEmpty(info.PM.Start.ToString()) && string.IsNullOrEmpty(info.PM.End.ToString()))
                {
                    is_checked = false;
                }
            }

            XmlNode node = xdoc.CreateElement(Enum.GetName(typeof(DayOfWeek), dow).ToLower());
            attr = xdoc.CreateAttribute("checked");
            attr.Value = (is_checked) ? "1" : "0";
            node.Attributes.Append(attr);
            root.AppendChild(node);

            XmlNode am = xdoc.CreateElement("am");
            attr = xdoc.CreateAttribute("start");
            attr.Value = info.AM.Start.ToString();
            am.Attributes.Append(attr);
            attr = xdoc.CreateAttribute("end");
            attr.Value = info.AM.End.ToString();
            am.Attributes.Append(attr);
            node.AppendChild(am);

            XmlNode pm = xdoc.CreateElement("pm");
            attr = xdoc.CreateAttribute("start");
            attr.Value = info.PM.Start.ToString();
            pm.Attributes.Append(attr);
            attr = xdoc.CreateAttribute("end");
            attr.Value = info.PM.End.ToString();
            pm.Attributes.Append(attr);
            node.AppendChild(pm);

            return node;
        }

        public IEnumerator<KeyValuePair<DayOfWeek, StaffTimeInfo>> GetEnumerator()
        {
            return _Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        public override string ToString()
        {
            string result = string.Empty;
            Dictionary<string, List<StaffTimeInfo>> temp = new Dictionary<string, List<StaffTimeInfo>>();
            var query = from StaffTimeInfo i in _Items.Values where i.Checked select i;
            if (query.Count() > 0)
            {
                foreach (StaffTimeInfo info in query)
                {
                    string time = info.TimeRangeToString();
                    if (!temp.ContainsKey(time))
                        temp.Add(time, new List<StaffTimeInfo>());
                    temp[time].Add(info);
                }

                Dictionary<string, List<StaffTimeInfo>>.Enumerator en = temp.GetEnumerator();
                while (en.MoveNext())
                {
                    result += "<div>";
                    result += GetDays(en.Current.Value) + " ";
                    result += en.Current.Key;
                    result += "</div>";
                }
            }
            return result;
        }

        public string GetDays(List<StaffTimeInfo> infos)
        {
            string result = string.Empty;
            var query = from StaffTimeInfo i in infos where i.Checked select i;
            StaffTimeInfo prev = null;
            foreach (StaffTimeInfo info in query)
            {
                if (string.IsNullOrEmpty(result))
                {
                    result += info.DayOfWeekToShortString();
                }
                else
                {
                    if ((int)info.DayOfWeek == ((int)prev.DayOfWeek) + 1)
                        result += (result.EndsWith("-")) ? string.Empty : "-";
                    else
                        result += ((result.EndsWith("-")) ? prev.DayOfWeekToShortString() : string.Empty) + "," + info.DayOfWeekToShortString();
                }

                prev = info;
            }

            if (!string.IsNullOrEmpty(result))
                if (result.EndsWith("-"))
                    result += prev.DayOfWeekToShortString();

            return result;
        }
    }
}

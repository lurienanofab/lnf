using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace LNF.Help
{
    public class StaffTimeInfoCollection : IEnumerable<KeyValuePair<DayOfWeek, StaffTimeInfo>>
    {
        private XDocument _xdoc;

        public StaffTimeInfoCollection() : this(string.Empty) { }

        public StaffTimeInfoCollection(string xml)
        {
            Init(xml);
        }

        public StaffTimeInfo this[DayOfWeek index]
        {
            get
            {
                var result = new StaffTimeInfo(index);

                string d = Enum.GetName(typeof(DayOfWeek), index).ToLower();
                var node = _xdoc.Element("staff_time").Element(d);

                if (node != null)
                {
                    result.Checked = node.Attribute("checked").Value == "1";
                    var am = node.Element("am");
                    result.AM.Start = new StaffTimeValue(am.Attribute("start").Value);
                    result.AM.End = new StaffTimeValue(am.Attribute("end").Value);
                    var pm = node.Element("pm");
                    result.PM.Start = new StaffTimeValue(pm.Attribute("start").Value);
                    result.PM.End = new StaffTimeValue(pm.Attribute("end").Value);
                }
                else
                {
                    result.Checked = false;
                    result.AM.Start = new StaffTimeValue();
                    result.AM.End = new StaffTimeValue();
                    result.PM.Start = new StaffTimeValue();
                    result.PM.End = new StaffTimeValue();
                }

                return result;
            }
            set
            {
                var node = GetItem(index);
                node.Attribute("checked").Value = value.Checked ? "0" : "1";
                node.Element("am").Attribute("start").Value = value.AM.Start.ToString();
                node.Element("am").Attribute("end").Value = value.AM.End.ToString();
                node.Element("pm").Attribute("start").Value = value.AM.Start.ToString();
                node.Element("pm").Attribute("end").Value = value.AM.End.ToString();
            }
        }

        /// <summary>
        /// Gets the hours_text element value. A new element is created if it does not exist.
        /// </summary>
        public string[] GetHoursText()
        {
            string[] result;

            var node = _xdoc.Element("staff_time").Element("hours_text");

            if (node != null)
            {
                if (node.Elements("line").Any())
                    result = node.Elements("line").Select(x => x.Value).ToArray();
                else
                    result = new[] { node.Value };
            }
            else
            {
                result = ConvertToHoursText();
            }

            return result;
        }

        /// <summary>
        /// Sets the hours_text element value. A new element is created if it does not exist.
        /// </summary>
        public void SetHoursText(string[] lines)
        {
            var node = _xdoc.Element("staff_time").Element("hours_text");

            if (node == null)
            {
                node = new XElement("hours_text");
                _xdoc.Element("staff_time").Add(node);
            }

            node.Value = string.Empty;
            node.Elements("line").Remove();

            bool skip = lines == null
                || lines.Length == 0
                || !lines.Any(x => !string.IsNullOrEmpty(x));
          
            if (!skip)
            {
                foreach (var line in lines)
                {
                    node.Add(new XElement("line", line));
                }
            }

            // we are using a free text field now so clear these
            Clear(DayOfWeek.Monday);
            Clear(DayOfWeek.Tuesday);
            Clear(DayOfWeek.Wednesday);
            Clear(DayOfWeek.Thursday);
            Clear(DayOfWeek.Friday);
            Clear(DayOfWeek.Saturday);
            Clear(DayOfWeek.Sunday);
        }

        private void Init(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                _xdoc = XDocument.Parse("<staff_time></staff_time>");
                GetItem(DayOfWeek.Sunday);
                GetItem(DayOfWeek.Monday);
                GetItem(DayOfWeek.Tuesday);
                GetItem(DayOfWeek.Wednesday);
                GetItem(DayOfWeek.Thursday);
                GetItem(DayOfWeek.Friday);
                GetItem(DayOfWeek.Saturday);
                SetHoursText(new string[0]);
            }
            else
            {
                _xdoc = XDocument.Parse(xml);
            }
        }

        /// <summary>
        /// Returns the element for the given week day. A new element is created if it does not exist.
        /// </summary>
        private XElement GetItem(DayOfWeek dow)
        {
            string d = Enum.GetName(typeof(DayOfWeek), dow).ToLower();
            var node = _xdoc.Element("staff_time").Element(d);

            if (node == null)
            {
                node = new XElement(d,
                        new XAttribute("checked", 0),
                        new XElement("am", new XAttribute("start", string.Empty), new XAttribute("end", string.Empty)),
                        new XElement("pm", new XAttribute("start", string.Empty), new XAttribute("end", string.Empty)));

                _xdoc.Element("staff_time").Add(node);
            }

            return node;
        }

        public XDocument ToXML() => _xdoc;

        public IList<StaffTimeInfo> ToList()
        {
            return new List<StaffTimeInfo>
            {
                this[DayOfWeek.Monday],
                this[DayOfWeek.Tuesday],
                this[DayOfWeek.Wednesday],
                this[DayOfWeek.Thursday],
                this[DayOfWeek.Friday],
                this[DayOfWeek.Saturday],
                this[DayOfWeek.Sunday]
            };
        }

        public IDictionary<DayOfWeek, StaffTimeInfo> ToDictionary()
        {
            return ToList().ToDictionary(k => k.DayOfWeek, v => v);
        }

        public IEnumerator<KeyValuePair<DayOfWeek, StaffTimeInfo>> GetEnumerator()
        {
            return ToDictionary().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private string[] ConvertToHoursText()
        {
            string[] lines;

            Dictionary<string, List<StaffTimeInfo>> temp = new Dictionary<string, List<StaffTimeInfo>>();

            var query = ToList().Where(x => x.Checked).ToList();

            if (query.Count > 0)
            {
                foreach (StaffTimeInfo info in query)
                {
                    string time = info.TimeRangeToString();
                    if (!temp.ContainsKey(time))
                        temp.Add(time, new List<StaffTimeInfo>());
                    temp[time].Add(info);
                }

                lines = temp.Select(kvp => GetDays(kvp.Value) + " " + kvp.Key).ToArray();
            }
            else
            {
                // this could happen if no days are checked
                lines = new string[0];
            }

            return lines;
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

        public void Clear(DayOfWeek dow)
        {
            var node = GetItem(dow);
            node.Attribute("checked").Value = "0";
            node.Element("am").Attribute("start").Value = string.Empty;
            node.Element("am").Attribute("end").Value = string.Empty;
            node.Element("pm").Attribute("start").Value = string.Empty;
            node.Element("pm").Attribute("end").Value = string.Empty;
        }
    }
}

using LNF.CommonTools;
using LNF.Data;
using LNF.DataAccess;
using System.Xml;

namespace LNF.Impl.Repository.Data
{
    public class ClientPreference : IClientPreference, IDataItem
    {
        public virtual int ClientPreferenceID { get; set; }
        public virtual int ClientID { get; set; }
        public virtual string Preferences { get; set; }
        public virtual string ApplicationName { get; set; }

        public ClientPreference()
        {
            Preferences = "<preferences/>";
        }

        protected virtual XmlDocument GetPreferencesXml()
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(Preferences);
            return xdoc;
        }

        protected virtual XmlNode GetPreferenceNode(XmlDocument xdoc, string name)
        {
            XmlNode node = xdoc.SelectSingleNode("/preferences/add[@name='" + name + "']");
            return node;
        }

        protected virtual XmlNode CreateChildNode(XmlElement parent, string name)
        {
            XmlElement child = parent.OwnerDocument.CreateElement("add");
            XmlAttribute attr;

            attr = parent.OwnerDocument.CreateAttribute("name");
            attr.Value = name;
            child.Attributes.Append(attr);

            attr = parent.OwnerDocument.CreateAttribute("value");
            attr.Value = string.Empty;
            child.Attributes.Append(attr);

            parent.AppendChild(child);
            Preferences = parent.OwnerDocument.OuterXml;
            return child;
        }

        public virtual T GetPreference<T>(string name, T defval)
        {
            string result = string.Empty;
            XmlDocument xdoc = GetPreferencesXml();
            XmlNode node = GetPreferenceNode(xdoc, name);
            if (node == null)
                node = SetPreference(name, defval);
            result = node.Attributes["value"].Value;
            return Utility.ConvertTo(result, defval);
        }

        public virtual XmlNode SetPreference(string name, object value)
        {
            XmlDocument xdoc = GetPreferencesXml();
            XmlNode node = GetPreferenceNode(xdoc, name);
            if (node == null)
                node = CreateChildNode(xdoc.DocumentElement, name);
            node.Attributes["value"].Value = value.ToString();
            Preferences = xdoc.OuterXml;
            return node;
        }
    }
}

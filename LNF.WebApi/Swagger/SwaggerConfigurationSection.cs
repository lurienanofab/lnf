using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;

namespace LNF.WebApi.Swagger
{
    public class SwaggerConfigurationSection : ConfigurationSection
    {
        public static SwaggerConfigurationSection GetConfig()
        {
            var result = ConfigurationManager.GetSection("lnf/swagger");
            if (result == null) return null;
            else return (SwaggerConfigurationSection)result;
        }

        [ConfigurationProperty("title", IsRequired = true)]
        public string Title
        {
            get { return this["title"].ToString(); }
            set { this["title"] = value; }
        }

        [ConfigurationProperty("version", IsRequired = true)]
        public string Version
        {
            get { return this["version"].ToString(); }
            set { this["version"] = value; }
        }

        [ConfigurationProperty("xmlCommentsFiles", IsRequired = false)]
        public XmlCommentFileElementCollection XmlCommentsFiles
        {
            get { return (XmlCommentFileElementCollection)base["xmlCommentsFiles"]; }
            set { base["xmlCommentsFiles"] = value; }
        }

        public IEnumerable<string> GetXmlCommentsPaths()
        {
            if (XmlCommentsFiles == null)
                return new string[] { };
            else
                return XmlCommentsFiles;
        }
    }

    [ConfigurationCollection(typeof(StringElement))]
    public class XmlCommentFileElementCollection : ConfigurationElementCollection, IEnumerable<string>
    {
        public StringElement this[int index]
        {
            get
            {
                return (StringElement)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);
                BaseAdd(index, value);
            }
        }

        public new StringElement this[string key]
        {
            get { return (StringElement)BaseGet(key); }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new StringElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((StringElement)element).Value;
        }

        IEnumerator<string> IEnumerable<string>.GetEnumerator()
        {
            var enumerator = base.GetEnumerator();
            while (enumerator.MoveNext())
            {
                yield return ((StringElement)enumerator.Current).Value;
            }
        }
    }

    public class StringElement : ConfigurationElement
    {
        private string _Value;

        public string Value { get { return _Value; } }

        protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
        {
            _Value = (string)reader.ReadElementContentAs(typeof(string), null);
        }
    }
}

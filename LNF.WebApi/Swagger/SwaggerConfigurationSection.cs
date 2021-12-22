using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Hosting;

namespace LNF.WebApi.Swagger
{
    public class SwaggerConfigurationSection : ConfigurationSection
    {
        public static SwaggerConfigurationSection GetConfig()
        {
            var result = ConfigurationManager.GetSection("lnf/swagger");
            if (result == null) return null;
            else return result as SwaggerConfigurationSection;
        }

        [ConfigurationProperty("title", IsRequired = true)]
        public string Title => this["title"] as string;

        [ConfigurationProperty("version", IsRequired = true)]
        public string Version => this["version"] as string;

        [ConfigurationProperty("xmlCommentsFiles", IsRequired = false)]
        public XmlCommentFileElementCollection XmlCommentsFiles => base["xmlCommentsFiles"] as XmlCommentFileElementCollection;

        public string[] GetXmlCommentsPaths()
        {
            if (XmlCommentsFiles == null)
                return new string[] { };
            else
                return XmlCommentsFiles.Select(MapPath).ToArray();
        }

        private string MapPath(XmlFileElement e) => HostingEnvironment.MapPath(e.Path);
    }

    [ConfigurationCollection(typeof(XmlFileElement))]
    public class XmlCommentFileElementCollection : ConfigurationElementCollection, IEnumerable<XmlFileElement>
    {
        public XmlFileElement this[int index] => BaseGet(index) as XmlFileElement;

        public new XmlFileElement this[string key] => BaseGet(key) as XmlFileElement;

        protected override ConfigurationElement CreateNewElement() => new XmlFileElement();

        protected override object GetElementKey(ConfigurationElement element) => (element as XmlFileElement).Path;

        IEnumerator<XmlFileElement> IEnumerable<XmlFileElement>.GetEnumerator()
        {
            var enumerator = base.GetEnumerator();
            while (enumerator.MoveNext())
            {
                yield return enumerator.Current as XmlFileElement;
            }
        }
    }

    public class XmlFileElement : ConfigurationElement
    {
        [ConfigurationProperty("path")]
        public string Path => base["path"] as string;
    }
}

using LNF.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace LNF.Impl.Repository.Data
{
    public class OAuthClientAudience : IDataItem
    {
        public virtual int OAuthClientAudienceID { get; set; }
        public virtual OAuthClient OAuthClient { get; set; }
        public virtual string ApplicationName { get; set; }
        public virtual string ApplicationDescription { get; set; }
        public virtual XElement Configuration { get; set; }
        public virtual string AudienceId { get; set; }
        public virtual string AudienceSecret { get; set; }
        public virtual DateTime CreatedDateTime { get; set; }
        public virtual bool Active { get; set; }
        public virtual bool Deleted { get; set; }

        public virtual bool IsAllowed(Uri uri)
        {
            string[] check = { uri.GetLeftPart(UriPartial.Path), "*" };
            bool result = Configuration.Element("redirects").Elements("add").Any(x => x.Attribute("key").Value == "uri" && check.Contains(x.Attribute("value").Value));
            return result;
        }

        public virtual bool IsOrigin(Uri uri)
        {
            return Configuration.Element("origins").Elements("add").Any(x => x.Attribute("key").Value == "uri" && x.Attribute("value").Value == uri.ToString());
        }

        public virtual string[] Redirects()
        {
            return Configuration
                 .Element("root")
                 .Element("redirects")
                 .Descendants("add")
                 .Where(x => x.Attribute("key").Value == "uri")
                 .Select(x => x.Attribute("value").Value)
                 .ToArray();
        }

        public virtual string[] Origins()
        {
            return Configuration
                 .Element("root")
                 .Element("origins")
                 .Descendants("add")
                 .Where(x => x.Attribute("key").Value == "uri")
                 .Select(x => x.Attribute("value").Value)
                 .ToArray();
        }

        public static XElement CreateConfiguration(IEnumerable<string> redirectUris, IEnumerable<string> originUris)
        {
            XElement xdoc = XElement.Parse("<root><redirects/><origins/></root>");

            foreach (string s in redirectUris)
                xdoc.Element("redirects").Add(new XElement("add", new XAttribute("key", "uri"), new XAttribute("value", s)));

            foreach (string s in originUris)
                xdoc.Element("origins").Add(new XElement("add"), new XAttribute("key", "uri"), new XAttribute("value", s));

            return xdoc;
        }
    }
}

using LNF.Util.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace LNF.Impl.Util.Serialization
{
    public class XmlSerializer : ISerializer
    {
        public string SerializeObject(object obj)
        {
            var serializer = new global::System.Xml.Serialization.XmlSerializer(obj.GetType());
            using (MemoryStream ms = new MemoryStream())
            using (StreamReader reader = new StreamReader(ms))
            {
                serializer.Serialize(ms, obj);
                string result = reader.ReadToEnd();
                return result;
            }
        }

        public string Serialize<T>(T obj, params string[] properties) where T : new()
        {
            global::System.Xml.Serialization.XmlSerializer serializer;
            if (properties.Length > 0)
            { 
                XmlAttributeOverrides overrides = new XmlAttributeOverrides();
                foreach (var pinfo in obj.GetType().GetProperties().Where(x => !properties.Contains(x.Name)))
                    overrides.Add(typeof(T), pinfo.Name, new XmlAttributes() { XmlIgnore = true });
                serializer = new global::System.Xml.Serialization.XmlSerializer(typeof(T), overrides);
            }
            else
            {
                serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            }

            using (MemoryStream ms = new MemoryStream())
            using (StreamReader reader = new StreamReader(ms))
            {
                serializer.Serialize(ms, obj);
                ms.Position = 0;
                string result = reader.ReadToEnd();
                return result;
            }
        }

        public T Deserialize<T>(string value)
        {
            return (T)DeserializeObject(value, typeof(T));
        }

        public T DeserializeAnonymous<T>(string value, T anonymousObject)
        {
            IDictionary<string, object> dict = new Dictionary<string, object>();

            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(value);
            foreach (XmlNode node in xdoc.DocumentElement.ChildNodes)
            {
                var pinfo = anonymousObject.GetType().GetProperty(node.Name);
                if (pinfo != null)
                {
                    Type t = pinfo.PropertyType;
                    if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        if (string.IsNullOrEmpty(node.InnerText))
                            dict.Add(pinfo.Name, null);
                        else
                            dict.Add(pinfo.Name, Convert.ChangeType(node.InnerText, Nullable.GetUnderlyingType(t)));
                    }
                    else
                    { 
                        dict.Add(pinfo.Name, Convert.ChangeType(node.InnerText, t));
                    }
                }
            }
            T result = (T)Activator.CreateInstance(typeof(T), dict.Select(kvp => kvp.Value).ToArray());
            return result;
        }

        public object DeserializeObject(string value, Type t)
        {
            var serializer = new global::System.Xml.Serialization.XmlSerializer(t);
            using (TextReader reader = new StringReader(value))
            {
                object result = serializer.Deserialize(reader);
                return result;
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Web.UI;
using IronPython.Runtime;

namespace LNF.Scripting
{
    public static class ResultUtility
    {
        public static string CreateTag(Result result, string tagName, IDictionary<object, object> attributes = null, string innerHtml = null)
        {
            StringBuilder sb = new StringBuilder();
            using (HtmlTextWriter writer = new HtmlTextWriter(new StringWriter(sb)))
            {
                if (attributes != null)
                {
                    foreach (var kvp in attributes)
                        writer.AddAttribute(kvp.Key.ToString(), kvp.Value.ToString());
                }
                writer.RenderBeginTag(tagName);
                writer.Write(innerHtml);
                writer.RenderEndTag();
            }
            return sb.ToString();
        }

        public static IEnumerable<dynamic> GetData(Result result, string key)
        {
            if (string.IsNullOrEmpty(key)) key = "default";
            Data data = result.DataSet[key];
            return data.Items;
        }

        public static void SetData(Result result, object item, string key)
        {
            //item can be:
            //   1) a single normal object
            //   2) a collection of normal objects
            //   3) a single IDictionary object that contains key value pairs that are like property name/values
            //   4) a collection of IDictionary objects like #3

            IEnumerable list = item as IEnumerable;
            if (list == null)
                list = new object[] { item }; //item is #1, a single object so make a collection

            //list is now #2, #3, or #4

            IDictionary itemDictionary = list as IDictionary;
            if (itemDictionary != null)
                list = new IDictionary[] { itemDictionary }; //list is #3, a single IDictionary object so make a collection

            //list is now #2 or #4

            if (!result.DataSet.ContainsKey(key))
                result.DataSet.Add(key, new Data());

            Data data = result.DataSet[key];

            foreach (object i in list)
            {
                //list is a collection of objects that are either a normal object with properites or a
                //dictionary with key value pairs (key is the property name, value is the propety value)

                //in either case we want each item in list to be a dictionary with string keys and object values

                IDictionary<object, object> dict = new Dictionary<object, object>();

                itemDictionary = i as IDictionary;

                if (itemDictionary == null)
                {
                    //i is a normal object, not a dictionary
                    PropertyInfo[] pinfos = i.GetType().GetProperties();
                    foreach (PropertyInfo p in pinfos)
                    {
                        string name = p.Name;
                        object val = p.GetValue(i, null);
                        dict.Add(name, val);
                    }
                }
                else
                {
                    //i is a dictionary object
                    foreach (var k in itemDictionary.Keys)
                    {
                        string name = k.ToString();
                        object val = itemDictionary[k];
                        dict.Add(name, val);
                    }
                }
                
                data.AddItem(dict);
            }
        }

        public static void SetHeader(Result result, string fieldName, string displayText, string type, string key)
        {
            if (string.IsNullOrEmpty(displayText))
                displayText = fieldName;

            Header h = new Header() { FieldName = fieldName, DisplayText = displayText };

            switch (type)
            {
                case "int":
                    h.Type = typeof(int);
                    break;
                case "string":
                    h.Type = typeof(string);
                    break;
                default:
                    h.Type = typeof(object);
                    break;
            }

            if (!result.DataSet.ContainsKey(key))
                result.DataSet.Add(key, new Data());

            result.DataSet[key].AddHeader(h);
        }
    }
}

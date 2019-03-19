using LNF.Cache;
using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Data;
using Newtonsoft.Json;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

namespace LNF.Web
{
    public static class ListItemCollectionExtentions
    {
        public static void LoadPrivs(this ListItemCollection items)
        {
            var privs = DA.Current.Query<Priv>().ToList();

            foreach (var p in privs)
            {
                ListItem item = new ListItem
                {
                    Text = p.PrivType,
                    Value = ((int)p.PrivFlag).ToString()
                };

                items.Add(item);
            }
        }

        public static ClientPrivilege CalculatePriv(this ListItemCollection items)
        {
            int result = 0;

            foreach (ListItem chkPriv in items)
            {
                if (chkPriv.Selected)
                    result += int.Parse(chkPriv.Value);
            }

            return (ClientPrivilege)result;
        }

        public static int CalculateCommunities(this ListItemCollection items)
        {
            int result = 0;

            foreach (ListItem chk in items)
            {
                if (chk.Selected)
                    result += int.Parse(chk.Value);
            }

            return result;
        }
    }

    public static class HttpRequestExtensions
    {
        public static string GetDocumentContents(this HttpRequest request)
        {
            string documentContents;
            using (Stream receiveStream = request.InputStream)
            {
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    documentContents = readStream.ReadToEnd();
                }
            }
            return documentContents;
        }

        public static T GetDocumentContents<T>(this HttpRequest request)
        {
            var json = request.GetDocumentContents();
            return JsonConvert.DeserializeObject<T>(json);
        }
    }

    public static class HttpContextExtensions
    {
        public static ClientItem CurrentUser(this HttpContextBase context)
        {
            if (context.Items["CurrentUser"] == null)
            {
                context.Items["CurrentUser"] = ServiceProvider.Current.Data.GetClient(context.User.Identity.Name);
            }

            var result = (ClientItem)context.Items["CurrentUser"];

            return result;
        }

        public static void SetErrorID(this HttpContextBase context, string value)
        {
            context.Session["ErrorID"] = value;
        }

        public static string GetErrorID(this HttpContextBase context)
        {
            if (context.Session["ErrorID"] == null)
                return null;
            else
                return context.Session["ErrorID"].ToString();
        }

        public static void RemoveCacheData(this HttpContextBase context)
        {
            CacheManager.Current.RemoveValue(context.Cache().ToString("n"));
            context.Session.Remove("Cache");
        }

        public static Guid Cache(this HttpContextBase context)
        {
            if (context.Session["Cache"] == null)
                context.Session["Cache"] = Guid.NewGuid();

            return (Guid)context.Session["Cache"];
        }

        public static void CacheData(this HttpContextBase context, DataSet ds)
        {
            var key = context.Cache().ToString("n");
            CacheManager.Current.SetValue(key, ds, DateTimeOffset.Now.AddMinutes(10));
        }

        public static DataSet CacheData(this HttpContextBase context)
        {
            var key = context.Cache().ToString("n");

            var obj = CacheManager.Current.GetValue(key);

            if (obj == null) return null;
            else return (DataSet)obj;
        }
    }
}

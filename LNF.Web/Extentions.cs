using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Data;
using Newtonsoft.Json;
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
        public static ClientItem CurrentUser(this HttpContext context)
        {
            if (context.Items["CurrentUser"] == null)
            {
                context.Items["CurrentUser"] = ServiceProvider.Current.Data.GetClient(context.User.Identity.Name);
            }

            var result = (ClientItem)context.Items["CurrentUser"];

            return result;
        }
    }
}

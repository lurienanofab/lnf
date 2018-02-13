using LNF.Cache;
using LNF.CommonTools;
using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Data;
using LNF.Scheduler;
using LNF.Web.Mvc.UI;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
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
                ListItem item = new ListItem();
                item.Text = p.PrivType;
                item.Value = ((int)p.PrivFlag).ToString();
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

    public static class HtmlHelperExtensions
    {
        public static IHtmlString CreateSiteMenu(this HtmlHelper helper)
        {
            var currentUser = CacheManager.Current.CurrentUser;

            List<DropDownMenuItem> items = SiteMenu
                .Create(currentUser)
                .Select(x => new DropDownMenuItem()
                {
                    ID = x.MenuID,
                    ParentID = x.MenuParentID,
                    Target = x.TopWindow ? "_top" : x.NewWindow ? "_blank" : null,
                    Text = x.MenuText,
                    URL = x.MenuURL,
                    CssClass = x.MenuCssClass,
                    SortOrder = x.SortOrder
                }).ToList();

            items.Add(new DropDownMenuItem()
            {
                ID = 9999,
                ParentID = 0,
                Target = string.Empty,
                Text = string.Format("<div>Current User: {0}</div><div id=\"jclock\"></div>", currentUser.DisplayName),
                URL = string.Empty,
                CssClass = "menu-clock",
                SortOrder = 9999
            });

            DropDownMenu menu = new DropDownMenu(items, Utility.GetStaticUrl("images/lnfbanner.jpg"), new { @class = "site-menu" });

            string html = menu.Render();

            return new HtmlString(html);
        }
    }
}

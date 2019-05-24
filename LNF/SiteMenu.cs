using LNF.Models;
using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Data;
using System.Collections.Generic;
using System.Linq;

namespace LNF
{
    public class SiteMenu : SiteMenuBase
    {
        public SiteMenu(IClient client, string target) : base(GetMenuItems(), client, target) { }

        public override bool IsKiosk()
        {
            return ServiceProvider.Current.Context.UserHostAddress.StartsWith("192.168.1");
        }

        public override string GetLoginUrl()
        {
            return ServiceProvider.Current.Context.LoginUrl;
        }

        public override bool IsSecureConnection()
        {
            return ServiceProvider.Current.Context.GetRequestIsSecureConnection();
        }

        public static IEnumerable<IMenu> GetMenuItems()
        {
            return DA.Current.Query<Menu>()
                .Where(x => x.Active && !x.Deleted)
                .OrderBy(x => x.SortOrder)
                .CreateModels<IMenu>();
        }
    }
}

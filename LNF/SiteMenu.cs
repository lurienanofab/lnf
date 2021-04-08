using LNF.Data;
using System.Collections.Generic;

namespace LNF
{
    public class SiteMenu : SiteMenuBase
    {
        public SiteMenu(IClient client, string target, string loginUrl, bool isSecureConnection, string option)
            : base(client, target, loginUrl, isSecureConnection, option) { }

        protected override IEnumerable<IMenu> GetMenuItems()
        {
            return ServiceProvider.Current.Data.Menu.GetMenuItems();
        }
    }
}

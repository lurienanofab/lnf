using LNF.Models.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace LNF.Models
{
    public abstract class SiteMenuBase : IEnumerable<IMenu>
    {
        private IList<IMenu> _items;

        public IClient Client { get; }

        public string Target { get; set; }

        public SiteMenuBase(IEnumerable<IMenu> items, IClient client, string target)
        {
            if (items == null) throw new ArgumentNullException("items");
            else _items = items.ToList();

            Client = client ?? throw new ArgumentNullException("client");

            Target = target;

            SetLoginUrl();

            SetNavigateUrl();
        }

        public abstract bool IsKiosk();

        public abstract string GetLoginUrl();

        public abstract bool IsSecureConnection();

        public string GetTarget()
        {
            if (string.IsNullOrEmpty(Target))
                return "_self";
            else
                return Target;
        }

        private void SetLoginUrl()
        {
            var logout = _items.FirstOrDefault(x => x.IsLogout);
            if (logout != null)
                logout.MenuURL = GetLoginUrl();
        }

        private void SetNavigateUrl()
        {
            string prefix = IsSecureConnection() ? "https://" : "http://";

            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["AppServer"]))
                throw new Exception("AppSetting AppServer is required.");

            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["SchedServer"]))
                throw new Exception("AppSetting SchedServer is required.");

            var appServer = prefix + ConfigurationManager.AppSettings["AppServer"];
            var schedServer = prefix + ConfigurationManager.AppSettings["SchedServer"];

            foreach (var m in _items.Where(x => x.MenuURL != null))
            {
                var url = m.MenuURL.Replace("{AppServer}", appServer).Replace("{SchedServer}", schedServer);
                m.MenuURL = url;
            }
        }

        public IEnumerator<IMenu> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

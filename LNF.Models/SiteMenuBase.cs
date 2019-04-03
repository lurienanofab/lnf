using LNF.Models.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace LNF.Models
{
    public abstract class SiteMenuBase : IEnumerable<MenuItem>
    {
        private IEnumerable<MenuItem> _items;

        public IClient Client { get; }

        public string Target { get; set; }

        public SiteMenuBase(IEnumerable<MenuItem> items, IClient client, string target)
        {
            _items = items ?? throw new ArgumentNullException("items");

            Client = client ?? throw new ArgumentNullException("client");

            Target = target;

            SetLoginUrl();

            SetNavigateUrl();
        }

        public abstract bool IsKiosk();

        public abstract string GetLoginUrl();

        public abstract bool IsSecureConnection();

        public bool IsVisible(MenuItem item)
        {
            return MenuItem.IsVisible(Client, item.MenuPriv);
        }

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
                m.MenuURL = m.MenuURL.Replace("{AppServer}", appServer).Replace("{SchedServer}", schedServer);
        }

        public IEnumerator<MenuItem> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

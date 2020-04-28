using LNF.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace LNF
{
    public abstract class SiteMenuBase : IEnumerable<IMenu>
    {
        private IList<IMenu> _items;

        public IClient Client { get; }

        public string Target { get; }

        public string LoginUrl { get; }

        public bool IsSecureConnection { get; }

        public SiteMenuBase(IClient client, string target, string loginUrl, bool isSecureConnection)
        {
            Client = client ?? throw new ArgumentNullException("client");

            Target = target;

            LoginUrl = loginUrl;

            IsSecureConnection = isSecureConnection;

            _items = GetMenuItems().ToList();

            SetLoginUrl();

            SetNavigateUrl();
        }

        public string GetTarget(IMenu m)
        {
            if (m.TopWindow)
                return "_top";
            else if (m.NewWindow)
                return "_blank";
            else
            {
                if (string.IsNullOrEmpty(Target))
                    return "_self";
                else
                    return Target;
            }
        }

        protected abstract IEnumerable<IMenu> GetMenuItems();

        private void SetLoginUrl()
        {
            var logout = _items.FirstOrDefault(x => x.IsLogout);
            if (logout != null)
                logout.MenuURL = FormatUrl(LoginUrl, false);
        }

        private void SetNavigateUrl()
        {
            foreach (var m in _items.Where(x => x.MenuURL != null))
            {
                var url = FormatUrl(m.MenuURL, true);
                m.MenuURL = url;
            }
        }

        private string FormatUrl(string url, bool prependScheme)
        {
            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["AppServer"]))
                throw new Exception("AppSetting AppServer is required.");

            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["SchedServer"]))
                throw new Exception("AppSetting SchedServer is required.");

            string scheme = string.Empty;

            if (prependScheme)
                scheme = IsSecureConnection ? "https://" : "http://";

            var appServer = scheme + ConfigurationManager.AppSettings["AppServer"];
            var schedServer = scheme + ConfigurationManager.AppSettings["SchedServer"];

            return url
                .Replace("{AppServer}", appServer)
                .Replace("{SchedServer}", schedServer);
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

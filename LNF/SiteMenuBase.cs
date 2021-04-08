using LNF.CommonTools;
using LNF.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;

namespace LNF
{
    public abstract class SiteMenuBase : IEnumerable<IMenu>
    {
        private IList<IMenu> _items;

        public IClient Client { get; }

        public string Target { get; }

        public string LoginUrl { get; }

        public bool IsSecureConnection { get; }

        public string Option { get; }

        public SiteMenuBase(IClient client, string target, string loginUrl, bool isSecureConnection, string option)
        {
            Client = client ?? throw new ArgumentNullException("client");

            Target = target;

            LoginUrl = loginUrl;

            IsSecureConnection = isSecureConnection;

            Option = option;

            _items = GetMenuItems().ToList();

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
                {
                    if (Option == "UseViewLink")
                    {
                        // the link will be something like /sselonline?View=<url> so we always want to load in the top window
                        return "_top"; 
                    }
                    else
                    {
                        return Target;
                    }
                }
            }
        }

        protected abstract IEnumerable<IMenu> GetMenuItems();

        private void SetNavigateUrl()
        {
            foreach (var m in _items.Where(x => x.MenuURL != null))
            {
                m.MenuURL = FormatUrl(m.MenuURL);
                m.MenuURL = HandleOption(m);
            }
        }

        private string HandleOption(IMenu m)
        {
            string url = m.MenuURL;

            // if true we will target an iframe
            bool useTarget = !m.TopWindow && !m.NewWindow;

            if (useTarget && Option == "UseViewLink")
            {
                string format = Utility.GetRequiredAppSetting("ViewLinkFormat");
                bool urlEncode = bool.Parse(Utility.GetRequiredAppSetting("ViewLinkUrlEncode"));
                if (urlEncode) url = WebUtility.UrlEncode(url);
                url = string.Format(format, url);
            }

            return url;
        }

        private string FormatUrl(string url)
        {
            var appServer = Utility.GetRequiredAppSetting("AppServer");
            var schedServer = Utility.GetRequiredAppSetting("SchedServer");

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

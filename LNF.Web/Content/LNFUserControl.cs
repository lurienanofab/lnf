using LNF.Data;
using System;
using System.Web;
using System.Web.UI;

namespace LNF.Web.Content
{
    public class LNFUserControl : UserControl
    {
        public IProvider Provider => LNFPage.Provider;

        public HttpContextBase ContextBase => LNFPage.ContextBase;

        public IClient CurrentUser => LNFPage.CurrentUser;

        public LNFPage LNFPage
        {
            get
            {
                if (Page == null) return null;

                if (typeof(LNFPage).IsAssignableFrom(Page.GetType()))
                    return (LNFPage)Page;

                throw new Exception($"Cannot convert {Page.GetType().Name} to LNFPage.");
            }
        }
    }
}

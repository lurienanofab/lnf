using System.Web;
using System.Web.UI;

namespace LNF.Web.Content
{
    public class LNFUserControl : UserControl
    {
        public LNFUserControl()
        {
            ContextBase = new HttpContextWrapper(Context);
        }

        public HttpContextBase ContextBase { get; }

        public new LNFPage Page
        {
            get { return (LNFPage)base.Page; }
        }
    }
}

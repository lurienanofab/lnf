using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Web.Content
{
    public class LNFUserControl : System.Web.UI.UserControl
    {
        public new LNFPage Page
        {
            get { return (LNFPage)base.Page; }
        }
    }
}

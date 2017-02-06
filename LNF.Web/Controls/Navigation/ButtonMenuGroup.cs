using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace LNF.Web.Controls.Navigation
{
    [ParseChildren(true)]
    public class ButtonMenuGroup
    {
        public string Title { get; set; }
        public int GroupID { get; set; }
    }
}

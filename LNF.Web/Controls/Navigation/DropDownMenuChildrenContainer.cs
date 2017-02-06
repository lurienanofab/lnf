using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace LNF.Web.Controls.Navigation
{
    public class DropDownMenuChildrenContainer
    {
        private bool _Visible = true;

        public bool Visible
        {
            get { return _Visible; }
            set { _Visible = value; }
        }

        public Unit Width { get; set; }
    }
}

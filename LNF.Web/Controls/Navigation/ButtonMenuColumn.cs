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
    public class ButtonMenuColumn
    {
        private List<ButtonMenuGroup> _Groups = new List<ButtonMenuGroup>();
        private string _Header;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public List<ButtonMenuGroup> Groups
        {
            get { return _Groups; }
        }

        public string Header
        {
            get { return _Header; }
            set { _Header = value; }
        }
    }
}

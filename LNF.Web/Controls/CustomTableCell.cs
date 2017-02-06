using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LNF.Web.Controls
{
    [ToolboxData("<{0}:CustomTableCell runat=server></{0}:CustomTableCell>")]
    public class CustomTableCell : TableCell, IPostBackEventHandler
    {
        public DateTime CellDate { get; set; }
        public int ResourceID { get; set; }
        public int ReservationID { get; set; }
        public string MouseOverText { get; set; }
        public bool AutoPostBack { get; set; }

        public CustomTableCell()
        {
            AutoPostBack = true;
        }

        public event EventHandler Click;

        protected virtual void OnClick(EventArgs e)
        {
            if (Click != null)
                Click(this, e);
        }

        //private string _ClientPostBackScript;
        //protected override void OnPreRender(EventArgs e)
        //{
        //    _ClientPostBackScript = Page.ClientScript.GetPostBackEventReference(this, string.Empty);
        //    base.OnPreRender(e);
        //}

        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            if (AutoPostBack)
            {
                string onClick = Page.ClientScript.GetPostBackEventReference(this, string.Empty);
                writer.AddAttribute(HtmlTextWriterAttribute.Onclick, onClick);
                writer.AddAttribute(HtmlTextWriterAttribute.Name, UniqueID.ToString());
                writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID.ToString());
            }

            base.AddAttributesToRender(writer);
        }

        public void RaisePostBackEvent(string eventArgument)
        {
            OnClick(new EventArgs());
        }
    }
}
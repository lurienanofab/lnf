using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LNF.Web.Controls
{
    [DefaultProperty("Text"), ToolboxData("<{0}:ComboBox runat=server></{0}:ComboBox>")]
    public class ComboBox : WebControl, INamingContainer
    {
        protected TextBox txt = new TextBox();
        protected DropDownList ddl = new DropDownList();

        [Bindable(true)]
        public string Text
        {
            get { return txt.Text; }
            set { txt.Text = value; }
        }

        public object DataSource
        {
            get { return ddl.DataSource; }
            set { ddl.DataSource = value; }
        }

        public string DataValueField
        {
            get { return ddl.DataValueField; }
            set { ddl.DataValueField = value; }
        }

        public string DataTextField
        {
            get { return ddl.DataTextField; }
            set { ddl.DataTextField = value; }
        }

        public int SelectedIndex
        {
            get { return ddl.SelectedIndex; }
        }

        public string SelectedValue
        {
            get { return ddl.SelectedValue; }
        }

        public override void DataBind()
        {
            ddl.DataBind();
        }

        protected override void CreateChildControls()
        {
            Width = new Unit(200);
            Controls.Clear();
            Controls.Add(txt);
            Controls.Add(ddl);

            txt.AutoPostBack = true;
            txt.Width = new Unit(Width.Value - 20);
            //txt.Style["position"] = "absolute";
            txt.Style["z-Index"] = "2222";
            txt.Style["top"] = Style["top"];
            txt.Style["left"] = Style["left"];
            txt.TextChanged += TextBox_TextChanged;

            ddl.AutoPostBack = true;
            ddl.Width = Width;
            //ddl.Style["position"] = "absolute";
            ddl.Style["z-Index"] = "-1111";
            ddl.Style["top"] = Style["top"];
            ddl.Style["left"] = Style["left"];
            ddl.SelectedIndexChanged += DropDownList_SelectedIndexChanged;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            if (txt.Text.Length > 0)
            {
                var item = ddl.Items.FindByText(txt.Text);
                if (item == null)
                {
                    ddl.ClearSelection();
                    ddl.SelectedIndex = -1;
                    ddl.SelectedValue = string.Empty;
                }
                else
                {
                    ddl.SelectedValue = item.Value;
                }
            }
        }

        private void DropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            txt.Text = ddl.SelectedItem.Text;
        }
    }
}
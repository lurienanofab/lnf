using LNF.CommonTools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LNF.Web.Controls.Navigation
{
    public enum AccordionChildrenLayout
    {
        Flow = 0,
        Div = 1,
        Table = 2
    }

    [ParseChildren(true)]
    [ToolboxData("<{0}:Accordion runat=server></{0}:Accordion>")]
    public class Accordion : WebControl, INamingContainer
    {
        private List<AccordionParentItem> _ParentItems = new List<AccordionParentItem>();
        private bool _Bound = false;

        public AccordionChildrenLayout ChildrenLayoutMode { get; set; }

        public int ChildrenTableColumnCount { get; set; }

        public string ChildrenTableCssClass { get; set; }

        public bool Collapsible { get; set; }

        public bool AutoHeight { get; set; }

        public DataTable DataSource { get; set; }

        public int SelectedIndex
        {
            get
            {
                if (ViewState["SelectedIndex"] == null) ViewState["SelectedIndex"] = -1;
                return (int)ViewState["SelectedIndex"];
            }
            set
            {
                ViewState["SelectedIndex"] = value;
            }
        }

        public Accordion()
        {
            this.Collapsible = true;
            this.AutoHeight = true;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
        [PersistenceMode(PersistenceMode.InnerDefaultProperty)]
        public List<AccordionParentItem> ParentItems
        {
            get { return _ParentItems; }
        }

        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Div;
            }
        }

        public bool Bound { get { return _Bound; } }

        protected override void RenderContents(HtmlTextWriter output)
        {
            if (!this.Visible) return;

            this.Page.ClientScript.RegisterStartupScript(typeof(Page), this.ID + "_script", this.Script(), true);

            foreach (AccordionParentItem p in _ParentItems)
            {
                p.Parent = this;
                p.Render(output);
            }
        }

        public override void DataBind()
        {
            _Bound = false;

            if (this.DataSource == null) throw new NullReferenceException("DataSource cannot be null");

            if (!this.DataSource.Columns.Contains("MenuText"))
            {
                throw new DataException("DataSource must contain the column \"MenuText\"");
            }

            _ParentItems.Clear();

            DataRow[] parents = this.DataSource.Select("MenuParentID = 0");

            foreach (DataRow pdr in parents)
            {
                AccordionParentItem p = new AccordionParentItem();
                if (pdr["MenuText"] == DBNull.Value) throw new DataException("Column \"MenuText\" cannot be null.");
                p.Text = pdr["MenuText"].ToString();
                p.NavigateURL = Utility.ConvertTo(pdr["MenuURL"], "#");
                p.Visible = Utility.ConvertTo(pdr["MenuVisible"], false);

                DataRow[] children = this.DataSource.Select("MenuParentID = " + pdr["MenuID"].ToString());
                foreach (DataRow cdr in children)
                {
                    AccordionChildItem c = new AccordionChildItem();
                    if (cdr["MenuText"] == DBNull.Value) throw new DataException("Column \"MenuText\" cannot be null.");
                    c.Text = cdr["MenuText"].ToString();
                    c.NavigateURL = Utility.ConvertTo(cdr["MenuURL"], "#");
                    c.Visible = Utility.ConvertTo(cdr["MenuVisible"], false);

                    p.ChildItems.Add(c);
                }

                _ParentItems.Add(p);
            }

            _Bound = true;

            base.DataBind();
        }

        public AccordionParentItem FindParent(string text)
        {
            AccordionParentItem result = null;

            foreach (AccordionParentItem p in _ParentItems)
            {
                if (p.Text == text)
                {
                    result = p;
                    break;
                }
            }

            return result;
        }

        public string Script()
        {
            StringBuilder sb = new StringBuilder();

            string active = "false";
            if (this.Collapsible)
            {
                if (this.SelectedIndex >= 0)
                {
                    active = this.SelectedIndex.ToString();
                }
            }
            else
            {
                active = Math.Max(0, this.SelectedIndex).ToString();
            }

            sb.AppendLine(string.Empty);
            sb.AppendLine("$(document).ready(function(){");
            sb.AppendLine("  $('#" + this.ClientID + "').accordion({");
            sb.AppendLine("    active: " + active + ",");
            sb.AppendLine("    autoHeight: " + this.AutoHeight.ToString().ToLower() + ",");
            sb.AppendLine("    clearStyle: false,");
            sb.AppendLine("    collapsible: " + this.Collapsible.ToString().ToLower());
            sb.AppendLine("  });");
            sb.AppendLine("  $('#" + this.ClientID + " h3.empty').unbind('click');");
            sb.Append("});");

            return sb.ToString();
        }
    }
}

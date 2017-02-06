using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Data;
using LNF.Cache;

namespace LNF.Web.Controls.Navigation
{
    [ParseChildren(true)]
    [ToolboxData("<{0}:ButtonMenu runat=server></{0}:ButtonMenu>")]
    public class ButtonMenu : CompositeDataBoundControl
    {
        private List<ButtonMenuColumn> _Columns = new List<ButtonMenuColumn>();
        private Authorization _Authorization;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public List<ButtonMenuColumn> Columns
        {
            get { return _Columns; }
        }

        public new Authorization DataSource
        {
            get { return _Authorization; }
            set { _Authorization = value; }
        }

        protected override HtmlTextWriterTag TagKey
        {
            get { return HtmlTextWriterTag.Div; }
        }

        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            writer.AddAttribute("class", "button-menu");
            base.AddAttributesToRender(writer);
        }

        public void LoadAuth()
        {
            List<Authorization.Group> groups = _Authorization.GroupsToList();
            if (groups.Count > 0)
            {
                _Columns = new List<ButtonMenuColumn>();
                foreach (Authorization.Group g in groups)
                {
                    ButtonMenuColumn bmc = new ButtonMenuColumn();
                    bmc.Header = g.GroupName;
                }
            }
        }

        private void CreateButtonMenu(out int count)
        {
            Table tbl = new Table();
            TableRow row = new TableRow();
            TableCell cell;
            count = 0;
            foreach (ButtonMenuColumn bmc in _Columns)
            {
                cell = new TableCell();
                cell.Style.Add("padding-right", "20px");
                cell.VerticalAlign = VerticalAlign.Top;

                foreach (ButtonMenuGroup bmg in bmc.Groups)
                {
                    Panel panGroup = new Panel();
                    panGroup.CssClass = "button-group";

                    System.Web.UI.HtmlControls.HtmlGenericControl div = new System.Web.UI.HtmlControls.HtmlGenericControl();
                    div.TagName = "div";
                    div.Attributes.Add("class", "button-group-header");
                    div.InnerHtml = bmg.Title;

                    panGroup.Controls.Add(div);

                    Panel panButtons = new Panel();
                    panButtons.Style.Add("padding-top", "5px");

                    DataView dv = this.DataSource.SelectAppPages(bmg.GroupID);
                    count = dv.Count;
                    foreach (DataRowView drv in dv)
                    {
                        Panel panBtn = new Panel();
                        panBtn.Style.Add("padding-bottom", "15px");

                        ButtonMenuItem bmi = new ButtonMenuItem(new Authorization.AppPage(drv));

                        panBtn.Controls.Add(bmi.CreateButton());

                        panButtons.Controls.Add(panBtn);
                    }

                    panGroup.Controls.Add(panButtons);

                    cell.Controls.Add(panGroup);
                }

                row.Cells.Add(cell);
            }

            tbl.Rows.Add(row);

            //Add the exit application button
            row = new TableRow();
            foreach (ButtonMenuColumn bmc in _Columns)
            {
                cell = new TableCell();
                cell.Style.Add("padding-right", "20px");
                cell.VerticalAlign = VerticalAlign.Top;
                row.Cells.Add(cell);
            }

            this.Controls.Add(tbl);
        }

        protected override int CreateChildControls(IEnumerable dataSource, bool dataBinding)
        {
            int count = 0;
            if (dataBinding) CreateButtonMenu(out count);
            return count;
        }

        protected void Nav_Command(object sender, CommandEventArgs e)
        {
            string arg = e.CommandArgument.ToString();
            if (!string.IsNullOrEmpty(arg))
            {
                if (e.CommandName == "exit")
                {
                    CacheManager.Current.RemoveCacheData();
                    CacheManager.Current.AbandonSession();
                }
                HttpContext.Current.Response.Redirect(arg);
            }
        }
    }
}

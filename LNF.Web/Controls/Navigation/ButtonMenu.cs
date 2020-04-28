using LNF.Authorization;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LNF.Web.Controls.Navigation
{
    [ParseChildren(true)]
    [ToolboxData("<{0}:ButtonMenu runat=server></{0}:ButtonMenu>")]
    public class ButtonMenu : CompositeDataBoundControl
    {
        public HttpContextBase ContextBase { get; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public List<ButtonMenuColumn> Columns { get; private set; } = new List<ButtonMenuColumn>();

        public new PageAuthorization DataSource { get; set; }

        protected override HtmlTextWriterTag TagKey
        {
            get { return HtmlTextWriterTag.Div; }
        }

        public ButtonMenu()
        {
            ContextBase = new HttpContextWrapper(Context);
        }

        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            writer.AddAttribute("class", "button-menu");
            base.AddAttributesToRender(writer);
        }

        public void LoadAuth()
        {
            List<PageAuthorization.Group> groups = DataSource.GroupsToList();
            if (groups.Count > 0)
            {
                Columns = new List<ButtonMenuColumn>();
                foreach (PageAuthorization.Group g in groups)
                {
                    ButtonMenuColumn bmc = new ButtonMenuColumn { Header = g.GroupName };
                }
            }
        }

        private void CreateButtonMenu(out int count)
        {
            Table tbl = new Table();
            TableRow row = new TableRow();
            TableCell cell;
            count = 0;
            foreach (ButtonMenuColumn bmc in Columns)
            {
                cell = new TableCell { VerticalAlign = VerticalAlign.Top };
                cell.Style.Add("padding-right", "20px");

                foreach (ButtonMenuGroup bmg in bmc.Groups)
                {
                    Panel panGroup = new Panel { CssClass = "button-group" };

                    var div = new System.Web.UI.HtmlControls.HtmlGenericControl() { TagName = "div", InnerHtml = bmg.Title };
                    div.Attributes.Add("class", "button-group-header");

                    panGroup.Controls.Add(div);

                    Panel panButtons = new Panel();
                    panButtons.Style.Add("padding-top", "5px");

                    DataView dv = DataSource.SelectAppPages(bmg.GroupID);
                    count = dv.Count;
                    foreach (DataRowView drv in dv)
                    {
                        Panel panBtn = new Panel();
                        panBtn.Style.Add("padding-bottom", "15px");

                        ButtonMenuItem bmi = new ButtonMenuItem(new PageAuthorization.AppPage(drv));

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
            foreach (ButtonMenuColumn bmc in Columns)
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
                    ContextBase.RemoveCacheData();
                    ContextBase.Session.Abandon();
                }
                HttpContext.Current.Response.Redirect(arg);
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LNF.Web.Controls
{
    [DefaultProperty("Tabs")]
    [ParseChildren(true, "Tabs")]
    [ToolboxData("<{0}:TabStrip runat=server></{0}:TabStrip>")]
    public class TabStrip : WebControl, INamingContainer
    {
        private List<Tab> arrTabs;
        private int curTabIndex;
        private Repeater rptTabStrip;

        public TabStrip()
        {
            rptTabStrip = new Repeater();
            rptTabStrip.ItemCreated += TabStrip_ItemCreated;
            rptTabStrip.ItemDataBound += TabStrip_ItemDataBound;
            rptTabStrip.ItemCommand += TabStrip_ItemCommand;
        }

        [Category("Behavior")]
        [Description("The Tabs collection")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor(typeof(TabCollectionEditor), typeof(UITypeEditor))]
        [PersistenceMode(PersistenceMode.InnerDefaultProperty)]
        public List<Tab> Tabs
        {
            get
            {
                if (arrTabs == null)
                    arrTabs = new List<Tab>();
                return arrTabs;
            }
        }

        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

        protected override void CreateChildControls()
        {
            BindTabStrip();
            Controls.Add(rptTabStrip);
        }

        protected override void Render(HtmlTextWriter output)
        {
            output.Write("<table cellspacing=0 cellpadding=0 width='100%' border=0><tr>" + Environment.NewLine);
            rptTabStrip.RenderControl(output);
            output.Write("<td style='border-bottom: solid 1px black;' width=100%>&nbsp;</td>");
            output.Write("</tr></table>" + Environment.NewLine);
        }

        private void BindTabStrip()
        {
            rptTabStrip.DataSource = Tabs;
            rptTabStrip.DataBind();
        }

        private void TabStrip_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                if (((Tab)e.Item.DataItem).Visible)
                {
                    TableCell tdSep = new TableCell();
                    tdSep.Attributes.Add("style", "border-bottom: solid 1px black;");
                    tdSep.Attributes.Add("width", "10");
                    tdSep.Text = "&nbsp;";
                    e.Item.Controls.Add(tdSep);

                    TableCell tdTab = new TableCell();
                    Button btn = new Button();
                    btn.ForeColor = Color.Navy;
                    btn.Style["font-family"] = "Tahoma,Sans-Serif";
                    btn.Style["font-weight"] = "bold";
                    btn.Style["font-size"] = "10pt";
                    btn.CausesValidation = false;
                    btn.Text = ((Tab)e.Item.DataItem).Caption;
                    tdTab.Controls.Add(btn);
                    e.Item.Controls.Add(tdTab);
                }
            }
        }

        private void TabStrip_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                if (((Tab)e.Item.DataItem).Visible)
                {
                    Button btnTab = (Button)e.Item.Controls[1].Controls[0];
                    if (e.Item.ItemIndex == curTabIndex)
                    {
                        btnTab.Style["background-color"] = "white";
                        btnTab.Style["cursor"] = "default";
                        btnTab.Style["border"] = "solid 1px black";
                        btnTab.Style["border-bottom"] = "none 0px white";
                        btnTab.Attributes.Add("onmouseover", "this.style.borderBottom='none 0px white'; this.style.backgroundColor='white';");
                        btnTab.Attributes.Add("onmouseout", "this.style.borderBottom='none 0px white'; this.style.backgroundColor='white';");
                    }
                    else
                    {
                        btnTab.Style["background-color"] = "lightsteelblue";
                        btnTab.Style["cursor"] = "hand";
                        btnTab.Style["border"] = "solid 1px black";
                        btnTab.Attributes.Add("onmouseover", "this.style.border='solid 1px black'; this.style.backgroundColor='white';");
                        btnTab.Attributes.Add("onmouseout", "this.style.border='solid 1px black'; this.style.backgroundColor='lightsteelblue';");
                    }
                }
            }
        }

        private void TabStrip_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            ChangeIndex(e.Item.ItemIndex);
        }

        public void ChangeIndex(int newIndex)
        {
            if (newIndex < 0 || newIndex >= Tabs.Count)
                newIndex = 0;

            curTabIndex = newIndex;
            BindTabStrip();

            SelectionChangedEventArgs eventArgs = new SelectionChangedEventArgs(newIndex);
            eventArgs.Caption = arrTabs[newIndex].Caption;
            eventArgs.ID = arrTabs[newIndex].Value;

            if (SelectionChanged != null)
                SelectionChanged(this, eventArgs);
        }
    }

    public class SelectionChangedEventArgs : EventArgs
    {
        public int TabPosition { get; set; }
        public string Caption { get; set; }
        public string ID { get; set; }

        public SelectionChangedEventArgs() : this(0) { }

        public SelectionChangedEventArgs(int tabPosition)
        {
            TabPosition = tabPosition;
        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Tab
    {
        [Category("Behavior"), DefaultValue(""), Description("Caption of Tab")]
        public string Caption { get; set; }

        [Category("Behavior"), DefaultValue(""), Description("Value of Tab")]
        public string Value { get; set; }

        [Category("Behavior"), DefaultValue(true), Description("Visibility of Tab")]
        public bool Visible { get; set; }

        public Tab() : this(string.Empty, string.Empty, true) { }

        public Tab(string caption, string value, bool visible)
        {
            Caption = caption;
            Value = value;
            Visible = visible;
        }
    }

    public class TabCollectionEditor : CollectionEditor
    {
        public TabCollectionEditor(Type type) : base(type) { }

        protected override bool CanSelectMultipleInstances()
        {
            return false;
        }

        protected override Type CreateCollectionItemType()
        {
            return typeof(Tab);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LNF.Web.Controls
{
    [ToolboxData("<{0}:PeriodPicker runat=server></{0}:PeriodPicker>")]
    public class PeriodPicker : WebControl, INamingContainer
    {
        public event EventHandler<PeriodChangedEventArgs> SelectedPeriodChanged;

        private DropDownList ddlYear;
        private DropDownList ddlMonth;

        public DateTime StartPeriod { get; set; }
        public DateTime EndPeriod { get; set; }
        public bool AutoPostBack { get; set; }

        public DateTime SelectedPeriod
        {
            get
            {
                EnsureChildControls();
                int y = Convert.ToInt32(ddlYear.SelectedValue);
                int m = Convert.ToInt32(ddlMonth.SelectedValue);
                return new DateTime(y, m, 1);
            }
            set
            {
                EnsureChildControls();
                if (ContainsPeriod(value))
                {
                    ddlYear.SelectedValue = value.Year.ToString();
                    ddlMonth.SelectedValue = value.Month.ToString();
                }
            }
        }

        public int SelectedYear
        {
            get
            {
                EnsureChildControls();
                return SelectedPeriod.Year;
            }
        }

        public int SelectedMonth
        {
            get
            {
                EnsureChildControls();
                return SelectedPeriod.Month;
            }
        }

        protected override string TagName
        {
            get { return "span"; }
        }

        protected override void CreateChildControls()
        {
            ddlYear = new DropDownList();
            ddlYear.ID = "ddlYear";
            ddlYear.CssClass = "year-select";
            ddlYear.SelectedIndexChanged += new EventHandler(HandleSelectedIndexChanged);

            ddlMonth = new DropDownList();
            ddlMonth.ID = "ddlMonth";
            ddlMonth.CssClass = "month-select";
            ddlMonth.SelectedIndexChanged += new EventHandler(HandleSelectedIndexChanged);

            if (StartPeriod == DateTime.MinValue)
                StartPeriod = DateTime.Parse("2003-01-01");
            if (EndPeriod == DateTime.MinValue)
            {
                DateTime nextPeriod = DateTime.Now.AddMonths(1);
                EndPeriod = new DateTime(nextPeriod.Year, nextPeriod.Month, 1);
            }

            List<dynamic> years = new List<dynamic>();
            List<dynamic> months = new List<dynamic>();

            DateTime p = StartPeriod;
            while (p <= EndPeriod)
            {
                if (years.FirstOrDefault(x => x.Value == p.Year) == null)
                    years.Add(new { Text = p.ToString("yyyy"), Value = p.Year });
                if (months.FirstOrDefault(x => x.Value == p.Month) == null)
                    months.Add(new { Text = p.ToString("MMMM"), Value = p.Month });
                p = p.AddMonths(1);
            }

            ddlYear.AutoPostBack = AutoPostBack;
            ddlYear.DataTextField = "Text";
            ddlYear.DataValueField = "Value";
            ddlYear.DataSource = years.OrderBy(x => x.Value);
            ddlYear.DataBind();

            ddlMonth.AutoPostBack = AutoPostBack;
            ddlMonth.DataTextField = "Text";
            ddlMonth.DataValueField = "Value";
            ddlMonth.DataSource = months.OrderBy(x => x.Value);
            ddlMonth.DataBind();

            DateTime prevPeriod = DateTime.Now.AddMonths(-1);

            if (ddlYear.Items.FindByValue(prevPeriod.Year.ToString()) != null)
            {
                if (ddlMonth.Items.FindByValue(prevPeriod.Month.ToString()) != null)
                {
                    ddlYear.SelectedValue = prevPeriod.Year.ToString();
                    ddlMonth.SelectedValue = prevPeriod.Month.ToString();
                }
            }

            Controls.Add(ddlYear);
            Controls.Add(new LiteralControl(Environment.NewLine));
            Controls.Add(ddlMonth);
        }

        public bool ContainsPeriod(DateTime p)
        {
            EnsureChildControls();
            if (ddlYear.Items.FindByValue(p.Year.ToString()) == null) return false;
            if (ddlMonth.Items.FindByValue(p.Month.ToString()) == null) return false;
            return true;
        }

        private void HandleSelectedIndexChanged(object sender, EventArgs e)
        {
            OnSelectedPeriodChanged(new PeriodChangedEventArgs(SelectedPeriod));
        }

        protected virtual void OnSelectedPeriodChanged(PeriodChangedEventArgs e)
        {
            if (SelectedPeriodChanged != null)
                SelectedPeriodChanged(this, e);
        }
    }

    public class PeriodChangedEventArgs : EventArgs
    {
        private DateTime _SelectedPeriod;

        internal PeriodChangedEventArgs(DateTime selectedPeriod)
        {
            _SelectedPeriod = selectedPeriod;
        }

        public DateTime SelectedPeriod { get { return _SelectedPeriod; } }
    }
}
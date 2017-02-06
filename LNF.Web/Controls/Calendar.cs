using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace LNF.Web.Controls
{
    public class Calendar : WebControl
    {
        public DateTime SelectedDate { get; set; }
        public DateTime SelectedMonth { get; set; }
        public string[] ColumnHeaders { get; set; }
        public string ReturnTo { get; set; }
        public IEnumerable<CalendarWeek> Weeks { get; set; }

        public Calendar() : base(HtmlTextWriterTag.Div) { }

        public override void RenderBeginTag(HtmlTextWriter writer)
        {
            writer.AddAttribute("data-date", SelectedDate.ToString("yyyy-MM-dd"));
            writer.AddAttribute("data-month", SelectedMonth.ToString("yyyy-MM-dd"));
            writer.AddAttribute("data-returnto", ReturnTo);
            writer.AddAttribute("data-headers", string.Join(",", ColumnHeaders));
            base.RenderBeginTag(writer);
        }

        protected override void OnLoad(EventArgs e)
        {
            if (ColumnHeaders == null)
                ColumnHeaders = new[] { "S", "M", "T", "W", "T", "F", "S" };

            if (SelectedDate == default(DateTime))
                SelectedDate = DateTime.Now.Date;
            else
                SelectedDate = SelectedDate.Date;

            if (SelectedMonth == default(DateTime))
                SelectedMonth = GetFirstOfMonth(SelectedDate);
            else
                SelectedMonth = GetFirstOfMonth(SelectedMonth);

            if (Weeks == null)
                Weeks = GenerateWeeks(SelectedMonth);
        }

        protected override void CreateChildControls()
        {
            HtmlGenericControl root = new HtmlGenericControl("div");
            root.Attributes.Add("class", "calendar-root");

            root.Controls.Add(CreateHeader());
            root.Controls.Add(CreateTable());

            Controls.Add(root);
        }

        private HtmlGenericControl CreateHeader()
        {
            HtmlGenericControl header = new HtmlGenericControl("div");
            header.Attributes.Add("class", "calendar-header");

            HtmlAnchor prevMonth = new HtmlAnchor();
            prevMonth.Attributes.Add("class", "month-prev");
            prevMonth.HRef = "#";
            header.Controls.Add(prevMonth);

            HtmlAnchor nextMonth = new HtmlAnchor();
            nextMonth.Attributes.Add("class", "month-next");
            nextMonth.HRef = "#";
            header.Controls.Add(nextMonth);

            HtmlGenericControl monthText = new HtmlGenericControl("div");
            monthText.Attributes.Add("class", "month-text");
            monthText.InnerHtml = SelectedMonth.ToString("MMMM yyyy");

            header.Controls.Add(monthText);

            return header;
        }

        private Table CreateTable()
        {
            Table table = new Table();
            table.CssClass = "calendar-table";

            TableRow row;

            row = new TableRow();
            row.TableSection = TableRowSection.TableHeader;

            foreach (string s in ColumnHeaders)
                row.Cells.Add(new TableHeaderCell() { Text = s });

            table.Rows.Add(row);

            foreach (CalendarWeek cw in Weeks)
            {
                row = new TableRow();
                row.TableSection = TableRowSection.TableBody;
                row.Cells.Add(CreateDayCell(cw.Sunday));
                row.Cells.Add(CreateDayCell(cw.Monday));
                row.Cells.Add(CreateDayCell(cw.Tuesday));
                row.Cells.Add(CreateDayCell(cw.Wednesday));
                row.Cells.Add(CreateDayCell(cw.Thursday));
                row.Cells.Add(CreateDayCell(cw.Friday));
                row.Cells.Add(CreateDayCell(cw.Saturday));
                table.Rows.Add(row);
            }

            return table;
        }

        private TableCell CreateDayCell(DateTime date)
        {
            TableCell cell = new TableCell();

            DateTime d = date.Date;

            if (d < SelectedMonth)
                cell.CssClass = "date-prev-month";
            else if (d >= SelectedMonth.AddMonths(1))
                cell.CssClass = "date-next-month";

            if (d == DateTime.Now.Date)
                cell.CssClass = "date-today";

            if (d == SelectedDate.Date)
                cell.CssClass = "date-selected";

            HyperLink link = new HyperLink();

            if (string.IsNullOrEmpty(ReturnTo))
                throw new InvalidOperationException("ReturnTo cannot be empty.");

            string navUrl = string.Format("~/CalendarController.ashx?Command=ChangeDate&Date={0:yyyy-MM-dd}&ReturnTo={1}", d, HttpUtility.UrlEncode(ReturnTo));

            link.NavigateUrl = navUrl;

            link.Text = d.Day.ToString();
            link.CssClass = "date";

            cell.Controls.Add(link);

            return cell;
        }

        private CalendarWeek CreateCalendarWeek(DateTime date)
        {
            CalendarWeek cw = new CalendarWeek();
            cw.Sunday = date;
            cw.Monday = date.AddDays(1);
            cw.Tuesday = date.AddDays(2);
            cw.Wednesday = date.AddDays(3);
            cw.Thursday = date.AddDays(4);
            cw.Friday = date.AddDays(5);
            cw.Saturday = date.AddDays(6);
            return cw;
        }

        private IEnumerable<CalendarWeek> GenerateWeeks(DateTime date)
        {
            List<CalendarWeek> result = new List<CalendarWeek>();

            DateTime fom = GetFirstOfMonth(date);

            //get the most recent Sunday before fom
            DateTime sd = fom.AddDays(-(int)fom.DayOfWeek);

            //check if the first day of the month is a Sunday, if so add another row for the last week of the previous month
            //because we always want to see some days from the the previous month
            if (sd == fom)
                sd = sd.AddDays(-7);

            DateTime prevMonth = fom.AddMonths(-1);
            DateTime nextMonth = fom.AddMonths(1);

            while (sd < nextMonth)
            {
                CalendarWeek cw = CreateCalendarWeek(sd);
                result.Add(cw);
                sd = sd.AddDays(7);
            }

            //check if the last day of the month is a saturday, if so add another row for the 1st week of the next month
            //because we always want to see some days from the next month
            DateTime lom = nextMonth.AddDays(-1);

            if (lom.DayOfWeek == DayOfWeek.Saturday)
            {
                CalendarWeek cw = CreateCalendarWeek(nextMonth);
                result.Add(cw);
            }

            return result;
        }

        DateTime GetFirstOfMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }
    }

    public class CalendarWeek
    {
        public DateTime Sunday { get; set; }
        public DateTime Monday { get; set; }
        public DateTime Tuesday { get; set; }
        public DateTime Wednesday { get; set; }
        public DateTime Thursday { get; set; }
        public DateTime Friday { get; set; }
        public DateTime Saturday { get; set; }
    }
}

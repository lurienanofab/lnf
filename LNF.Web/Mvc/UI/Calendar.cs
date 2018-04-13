using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using LNF.CommonTools;

namespace LNF.Web.Mvc.UI
{
    public class Calendar
    {
        public string Name { get; set; }
        public DateTime SelectedMonth { get; set; }
        public DateTime SelectedDate { get; set; }
        public object HtmlAttributes { get; set; }

        private TagBuilder self;

        public static readonly string[] ColumnHeaders = { "S", "M", "T", "W", "T", "F", "S" };

        private const string DATE_FORMAT = "MM/dd/yyyy";

        public static IHtmlString Render(DateTime selectedMonth, DateTime selectedDate)
        {
            TagBuilder monthText = new TagBuilder("div");
            monthText.AddCssClass("month-text");

            TagBuilder monthPrev = new TagBuilder("a");
            monthPrev.AddCssClass("month-prev");
            monthPrev.Attributes.Add("href", "#");

            TagBuilder monthNext = new TagBuilder("a");
            monthNext.AddCssClass("month-next");
            monthNext.Attributes.Add("href", "#");

            TagBuilder thead = new TagBuilder("thead");

            TagBuilder row = new TagBuilder("tr");

            foreach (string s in ColumnHeaders)
            {
                TagBuilder th = new TagBuilder("th");
                th.SetInnerText(s);
                row.InnerHtml += th.ToString();
            }

            thead.InnerHtml += row.ToString();

            TagBuilder tbody = new TagBuilder("tbody");

            //xxxxx set the month

            //make sure the date is really the first of the month
            var fom = Calendar.GetFirstOfMonth(selectedMonth);

            monthText.SetInnerText(fom.ToString("MMMM yyyy"));

            var prevMonth = Calendar.AddMonths(fom, -1);
            monthPrev.Attributes.Add("data-date", Calendar.FormatDate(prevMonth));

            var nextMonth = Calendar.AddMonths(fom, 1);
            monthNext.Attributes.Add("data-date", Calendar.FormatDate(nextMonth));

            //get the most recent Sunday before fom
            var sd = Calendar.AddDays(fom, -(int)fom.DayOfWeek);

            //check if the first day of the month is a Sunday, if so add another row for the last week of the previous month
            //because we always want to see some days from the the previous month
            if (sd == fom)
                sd = Calendar.AddDays(sd, -7);

            var now = DateTime.Now;
            var today = now.Date;

            Action<TagBuilder, DateTime> setCell = (cell, n) =>
            {
                TagBuilder a = Calendar.CreateCellLink(n);

                if (n < fom)
                    cell.AddCssClass("date-prev-month");
                else if (n >= nextMonth)
                    cell.AddCssClass("date-next-month");

                if (n == today)
                    cell.AddCssClass("date-today");

                if (n == selectedDate.Date)
                    cell.AddCssClass("date-selected");

                cell.InnerHtml += a;
            };

            while (sd < nextMonth)
            {
                row = new TagBuilder("tr");

                for (var x = 0; x < 7; x++)
                {
                    var cell = new TagBuilder("td");

                    var n = AddDays(sd, x);

                    setCell(cell, n);

                    row.InnerHtml += cell;
                }

                tbody.InnerHtml += row;

                sd = AddDays(sd, 7);
            }

            //check if the last day of the month is a saturday, if so add another row for the 1st week of the next month
            //because we always want to see some days from the next month
            var lom = nextMonth.AddDays(-1);

            if (lom.DayOfWeek == DayOfWeek.Saturday)
            {
                row = new TagBuilder("tr");
                for (var x = 0; x < 7; x++)
                {
                    var n = AddDays(nextMonth, x);

                    var cell = new TagBuilder("td");
                    setCell(cell, n);

                    row.InnerHtml += cell;
                }
                tbody.InnerHtml += row;
            }

            //put it all together

            TagBuilder header = new TagBuilder("div");
            header.AddCssClass("calendar-header");
            header.InnerHtml += monthPrev.ToString();
            header.InnerHtml += monthNext.ToString();
            header.InnerHtml += monthText.ToString();

            TagBuilder table = new TagBuilder("table");
            table.AddCssClass("calendar-table");
            table.InnerHtml += thead.ToString();
            table.InnerHtml += tbody.ToString();

            TagBuilder root = new TagBuilder("div");
            root.AddCssClass("calendar-root");
            root.InnerHtml += header.ToString();
            root.InnerHtml += table.ToString();

            return new HtmlString(root.ToString());
        }

        private void Setup()
        {
            self = new TagBuilder("div");
            self.AddCssClass("calendar");

            if (HtmlAttributes != null)
            {
                RouteValueDictionary attribs = HtmlHelper.AnonymousObjectToHtmlAttributes(HtmlAttributes);
                foreach (var kvp in attribs)
                {
                    if (kvp.Value == null)
                        self.Attributes.Add(kvp.Key, string.Empty);
                    else
                        self.Attributes.Add(kvp.Key, kvp.Value.ToString());
                }
            }
        }

        public static string FormatDate(DateTime d)
        {
            return d.ToString(DATE_FORMAT);
        }

        private static TagBuilder CreateCellLink(DateTime d)
        {
            TagBuilder a = new TagBuilder("a");
            a.Attributes.Add("href", "#");
            a.AddCssClass("date");
            a.SetInnerText(d.Day.ToString());
            a.Attributes.Add("data-date", FormatDate(d));
            return a;
        }

        public static DateTime GetFirstOfMonth(DateTime d)
        {
            return new DateTime(d.Year, d.Month, 1);
        }

        public static DateTime AddMonths(DateTime d, int months)
        {
            return d.AddMonths(months);
        }

        public static DateTime AddDays(DateTime d, int days)
        {
            return d.AddDays(days);
        }

        public IHtmlString Render()
        {
            if (string.IsNullOrEmpty(Name))
                throw new InvalidOperationException("Name is required.");

            Setup();

            self.InnerHtml += GetHidden().ToString(TagRenderMode.SelfClosing);
            self.InnerHtml += Calendar.Render(SelectedMonth, SelectedDate);

            return new HtmlString(self.ToString());
        }

        private string GetSelectedDate()
        {
            return SelectedDate.ToString(DATE_FORMAT);
        }

        private TagBuilder GetHidden()
        {
            TagBuilder hid = new TagBuilder("input");
            hid.AddCssClass("selected-date");
            hid.Attributes.Add("id", Name);
            hid.Attributes.Add("name", Name);
            hid.Attributes.Add("type", "hidden");
            hid.Attributes.Add("value", GetSelectedDate());

            return hid;
        }
    }
}

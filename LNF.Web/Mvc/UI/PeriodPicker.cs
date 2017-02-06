using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace LNF.Web.Mvc.UI
{
    public class PeriodPicker
    {
        public string Name {get;set;}
        public DateTime Period { get; set; }
        public int StartYear { get; set; }
        public int EndYear { get; set; }
        public string MonthFormat { get; set; }
        public bool AutoPostback { get; set; }
        public object HtmlAttributes { get; set; }

        public PeriodPicker(string name, DateTime period, int startYear = 2003, int endYear = 0, string monthFormat = "MMMM", bool autoPostback = false, object htmlAttributes = null)
        {
            Name = name;
            Period = period;
            StartYear = startYear;
            EndYear = endYear;
            MonthFormat = monthFormat;
            AutoPostback = autoPostback;
            HtmlAttributes = htmlAttributes;
        }

        public IHtmlString Render()
        {
            //default to previous period
            if (Period == DateTime.MinValue)
                Period = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1);

            StringBuilder sb = new StringBuilder();

            string onchange = "var parent=$(this).closest('.period-picker');parent.find('.period').val(parent.find('.period-picker-year').val()+'-'+parent.find('.period-picker-month').val()+'-01');if(parent.data('autopostback')===true){parent.closest('form').submit()};";

            sb.AppendLine(string.Format("<div class=\"period-picker form-inline\" data-autopostback=\"{0}\">", AutoPostback.ToString().ToLower()));
            sb.AppendLine(string.Format("<select class=\"period-picker-year form-control\" style=\"padding: 4px;\" onchange=\"{0}\">", onchange));
            if (EndYear == 0)
                EndYear = DateTime.Now.Year + 1;
            foreach (int y in Enumerable.Range(StartYear, EndYear - StartYear + 1))
            {
                sb.AppendLine(string.Format("<option{0}>{1}</option>", (y == Period.Year) ? " selected" : "", y));
            }
            sb.AppendLine("</select>");
            sb.AppendLine(string.Format("<select class=\"period-picker-month form-control\" style=\"padding: 4px;\" onchange=\"{0}\">", onchange));
            for (int m = 1; m < 13; m++)
            {
                DateTime temp = new DateTime(Period.Year, m, 1);
                sb.AppendLine(string.Format("<option{0} value=\"{1}\">{2}</option>", (m == Period.Month) ? " selected" : "", (m + 100).ToString().Substring(1), temp.ToString(MonthFormat)));
            }
            sb.AppendLine("</select>");
            sb.AppendLine(string.Format("<input type=\"hidden\" name=\"{0}\" value=\"{1}\" class=\"period\" />", Name, Period.ToString("yyyy-MM-dd")));
            sb.Append("</div>");

            return new HtmlString(sb.ToString());
        }
    }
}

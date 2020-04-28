using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace LNF.Meter
{
    public class ReportData
    {
        private ArrayList tableHeaders;
        private ArrayList dataHeaders;
        private List<Item> items;
        private StringBuilder html;
        private StringBuilder export;
        private List<DateTime> range;
        private IList<IMeterReport> reports;
        private double totalStart;
        private double totalEnd;
        private double totalUsage;
        private double totalCost;

        private ReportData()
        {
            tableHeaders = new ArrayList();
            dataHeaders = new ArrayList();
            html = new StringBuilder();
            export = new StringBuilder();
        }

        public static ReportData Create()
        {
            ReportData result = new ReportData();
            return result;
        }

        public ReportData AddDataHeader(string h)
        {
            dataHeaders.Add(h);
            return this;
        }

        public ReportData AddTableHeader(string h)
        {
            tableHeaders.Add(h);
            return this;
        }

        public ReportData Range(DateTime sdate, DateTime edate)
        {
            range = new List<DateTime>();
            items = new List<ReportData.Item>();
            DateTime d = sdate;
            while (d < edate)
            {
                if (d > DateTime.Now.Date.AddDays(-1)) break;
                range.Add(d);
                items.Add(new ReportData.Item() { Key = d.ToString("MM/dd"), Values = new List<double>() });
                d = d.AddDays(1);
            }
            return this;
        }

        public ReportData Reports(IEnumerable<IMeterReport> reports)
        {
            this.reports = reports.ToList();
            foreach (IMeterReport r in reports)
                dataHeaders.Add(r.ReportName);
            return this;
        }

        public ReportData ForEach(Func<IMeterReport, DateTime, double> fn)
        {
            totalStart = 0;
            totalEnd = 0;
            totalUsage = 0;
            totalCost = 0;
            foreach (IMeterReport report in reports)
            {
                html.AppendLine("<tr>");
                html.AppendLine(string.Format("<th style=\"text-align: right;\">{0}</th>", report.ReportName));
                export.Append(report.ReportName);
                int index = 0;
                double startValue = 0;
                double endValue = 0;
                foreach (DateTime d in range)
                {
                    double val = (fn == null) ? 0 : fn(report, d);
                    items[index].Values.Add(val);
                    if (startValue == 0 && val > 0)
                        startValue = val;
                    if (val > 0)
                        endValue = val;
                    index++;
                }

                totalStart += startValue;
                html.AppendLine(string.Format("<td style=\"text-align: right;\">{0}</td>", startValue));
                export.Append("," + startValue);

                totalEnd += endValue;
                html.AppendLine(string.Format("<td style=\"text-align: right;\">{0}</td>", endValue));
                export.Append("," + endValue);

                double usage = endValue - startValue;
                double lineCost = usage * report.UnitCost;
                totalCost += lineCost;
                html.AppendLine(string.Format("<td style=\"text-align: right;\">{0}</td>", usage));
                html.AppendLine(string.Format("<td style=\"text-align: right;\">${0}/kl</td>", report.UnitCost.ToString("#,##0.00")));
                html.AppendLine(string.Format("<td style=\"text-align: right;\">${0}</td>", lineCost.ToString("#,##0.00")));
                export.AppendLine("," + usage + "," + report.UnitCost.ToString("#,##0.00") + "," + lineCost.ToString("#,##0.00"));
                totalUsage += usage;
                html.AppendLine("</tr>");
            }
            return this;
        }

        public ReportData.ListResult List()
        {
            if (reports.Count == 0)
            {
                return new ReportData.ListResult()
                {
                    Data = new List<ArrayList>(),
                    Html = "<div class=\"nodata\">No report items were found.</div>",
                    Export = "No report items were found."
                };
            }

            List<ArrayList> data = new List<ArrayList>
            {
                dataHeaders
            };
            data.AddRange(items.Select(x => x.ToArrayList()));

            StringBuilder htmlResult = new StringBuilder();
            StringBuilder exportResult = new StringBuilder();
            string comma = string.Empty;

            htmlResult.AppendLine("<table class=\"grid\">");
            htmlResult.AppendLine("<tr>");
            foreach (string h in tableHeaders)
            {
                htmlResult.AppendLine(string.Format("<th>{0}</th>", h));
                exportResult.AppendFormat("{0}{1}", comma, h);
                comma = ",";
            }
            htmlResult.AppendLine("</tr>");
            exportResult.Append(Environment.NewLine);

            htmlResult.Append(html.ToString());
            exportResult.Append(export.ToString());

            htmlResult.AppendLine("<tr>");
            htmlResult.AppendLine("<th style=\"text-align: right;\">Total</th>");
            htmlResult.AppendLine(string.Format("<td style=\"text-align: right;\">{0}</td>", totalStart.ToString("#,##0.##")));
            htmlResult.AppendLine(string.Format("<td style=\"text-align: right;\">{0}</td>", totalEnd.ToString("#,##0.##")));
            htmlResult.AppendLine(string.Format("<td style=\"text-align: right;\">{0}</td>", totalUsage.ToString("#,##0.##")));
            htmlResult.AppendLine("<td style=\"text-align: right;\">&nbsp;</td>");
            htmlResult.AppendLine(string.Format("<td style=\"text-align: right;\">${0}</td>", totalCost.ToString("#,##0.00")));
            htmlResult.AppendLine("</tr>");
            htmlResult.AppendLine("</table>");

            return new ReportData.ListResult()
            {
                Data = data,
                Html = htmlResult.ToString(),
                Export = exportResult.ToString()
            };
        }

        public class ListResult
        {
            public List<ArrayList> Data { get; set; }
            public string Html { get; set; }
            public string Export { get; set; }
        }

        public class Item
        {
            public string Key { get; set; }
            public IList<double> Values { get; set; }

            public ArrayList ToArrayList()
            {
                ArrayList result = new ArrayList
                {
                    Key
                };
                result.AddRange(Values.ToArray());
                return result;
            }
        }
    }
}

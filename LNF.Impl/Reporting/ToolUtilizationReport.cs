using LNF.CommonTools;
using LNF.Impl.Repository;
using LNF.Impl.Repository.Scheduler;
using LNF.Reporting;
using NHibernate;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace LNF.Impl.Reporting
{
    public class ToolUtilizationReport : DefaultReport<ResourceCriteria>
    {
        private IList<ActivityData> headers;
        private string[] procTechs;

        protected ISession Session { get; }

        public ToolUtilizationReport(ISession session)
        {
            Session = session;
        }

        public override void WriteCriteria(StringBuilder sb)
        {
            Criteria.CreateWriter(sb)
                .WriteBeginTag("div")
                .WriteText("Select starting period:\n")
                .WriteYearSelect(2003)
                .WriteMonthSelect()
                .WriteBeginTag("span", new { @style = "margin-left: 20px;" })
                .WriteText("Number of months:\n")
                .WriteTextbox("MonthCount", "1", new { @class = "month-count-textbox", @style = "width: 30px;" })
                .WriteEndTag()
                .WriteEndTag()
                .WriteBeginTag("div")
                .WriteText("Stats based on:\n")
                .WriteRadios("StatsBasedOn", new GenericListItem[]
                    {
                        new GenericListItem("charged", "Charged", true),
                        new GenericListItem("scheduled", "Scheduled"),
                        new GenericListItem("actual", "Actual")
                    }, new { @class = "stats-based-on-radios" })
                .WriteEndTag()
                .WriteBeginTag("div", new { @style = "margin-top: 5px;" })
                .WriteBeginTag("div")
                .WriteCheckbox("IncludeForgiven", new GenericListItem("include_forgiven", "Include Forgiven"), new { @class = "include-forgiven-checkbox" })
                .WriteEndTag()
                .WriteBeginTag("div")
                .WriteCheckbox("ShowPercent", new GenericListItem("show_percent", "Show Percent of Total"), new { @class = "show-percent-checkbox" })
                .WriteEndTag()
                .WriteEndTag()
                .WriteBeginTag("div", new { @style = "margin-top: 10px;" })
                .WriteButton("RunReport", "Run Report", new { @class = "run-button" })
                .WriteButton("ExportReport", "Export", new { @class = "export-button" })
                .WriteEndTag();
        }

        public override string Key
        {
            get { return "tool-utilization-report"; }
        }

        public override string Title
        {
            get { return "Tool Utilization Report"; }
        }

        public override string CategoryName
        {
            get { return "Resource Reports"; }
        }

        public override GenericResult Execute(ResultType resultType)
        {
            GenericResult result = new GenericResult { Success = true, Message = string.Empty, Data = null };
            try
            {
                switch (resultType)
                {
                    case ResultType.Ajax:
                        result.Data = GetSetupData(GetToolUtilizationData());
                        break;
                    case ResultType.DataTables:
                        result.Data = GetReportData(GetToolUtilizationData());
                        break;
                    default:
                        result.Success = false;
                        result.Message = "Invalid result type.";
                        break;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }

        public DataTable GetToolUtilizationData()
        {
            return Session.Command(CommandType.Text)
                .Param("StartPeriod", Criteria.Period)
                .Param("EndPeriod", Criteria.Period.AddMonths(Criteria.GetValue("monthCount", 1)))
                .Param("IncludeForgiven", Criteria.GetValue("includeForgiven", false))
                .FillDataTable("SELECT * FROM Reporting.dbo.udf_ToolUtilizationReport(@StartPeriod, @EndPeriod, @IncludeForgiven)");
        }

        public string[] GetProcTechNames(IList<ProcessTech> list)
        {
            List<string> ptnames = new List<string>();
            foreach (string name in list.OrderBy(x => x.ProcessTechName).Select(x => x.ProcessTechName))
            {
                if (ptnames.FirstOrDefault(x => x == name) == null)
                {
                    ptnames.Add(name);
                }
            }
            return ptnames.ToArray();
        }

        public IList<ResourceData> GetResources()
        {
            IList<ProcessTech> allpt = Session.Query<ProcessTech>().ToList();
            IList<Resource> resources = Session.Query<Resource>().Where(x => x.IsActive).ToList();
            IList<Activity> activities = Session.Query<Activity>().Where(x => x.IsActive).OrderBy(x => x.ActivityName).ToList();
            IList<ResourceData> result = new List<ResourceData>();

            headers = activities
                .Select(x => new ActivityData { ActivityID = x.ActivityID, ActivityName = x.ActivityName, ChargeDuration = 0, SchedDuration = 0, ActDuration = 0 })
                .OrderBy(x => x.ActivityName)
                .ToList();

            procTechs = GetProcTechNames(allpt);

            foreach (Resource r in resources)
            {
                ProcessTech pt = allpt.FirstOrDefault(x => x.ProcessTechID == r.ProcessTech.ProcessTechID);
                result.Add(new ResourceData
                {
                    ResourceID = r.ResourceID,
                    ProcessTechID = r.ProcessTech.ProcessTechID,
                    LabID = r.ProcessTech.Lab.LabID,
                    ResourceName = r.ResourceName,
                    ProcessTechName = pt.ProcessTechName,
                    Activities = headers.ToList()
                });
            }

            return result
                .OrderBy(x => x.ProcessTechName)
                .ThenBy(x => x.ResourceName)
                .ToList();
        }

        public ArrayList GetColumns()
        {
            if (headers == null) GetResources();

            ArrayList columns = new ArrayList
            {
                new { sTitle = "ResourceID", bVisible = false },
                new { sTitle = "ProcessTechID", bVisible = false },
                new { sTitle = "LabID", bVisible = false },
                new { sTitle = "Resource" },
                new { sTitle = "Process Tech" }
            };

            foreach (ActivityData act in headers)
            {
                columns.Add(new { sTitle = act.ActivityName, sClass = "numeric-column" });
            }

            columns.Add(new { sTitle = "Total", sClass = "numeric-column" });

            return columns;
        }

        public object GetSetupData(DataTable table)
        {
            ArrayList columns = GetColumns();

            return new
            {
                aoColumns = columns,
                activities = headers,
                procTechs,
                tableColumns = columns.Count - 3,
                dataColumns = columns.Count
            };
        }

        public object GetReportData(DataTable table)
        {
            string statsBasedOn = Criteria.GetValue("statsBasedOn", "charged");
            bool showPercent = Criteria.GetValue("showPercent", false);
            ArrayList rows = new ArrayList();

            IList<ResourceData> resources = GetResources();

            foreach (ResourceData r in resources)
            {
                foreach (ActivityData a in r.Activities)
                {
                    DataRow[] drows = table.Select($"ResourceID = {r.ResourceID} AND ActivityID = {a.ActivityID}");
                    if (drows.Length > 0)
                    {
                        DataRow dr = drows[0];
                        ToolUtilizationItem item = new ToolUtilizationItem(dr);
                        a.ChargeDuration = item.ChargeDuration;
                        a.SchedDuration = item.SchedDuration;
                        a.ActDuration = item.ActDuration;
                    }
                }
                rows.Add(r.ToArray(statsBasedOn));
            }

            ArrayList columns = GetColumns();

            return new { aaData = rows };
        }

        public class ResourceData
        {
            public int ResourceID { get; set; }
            public int ProcessTechID { get; set; }
            public int LabID { get; set; }
            public string ResourceName { get; set; }
            public string ProcessTechName { get; set; }
            public List<ActivityData> Activities { get; set; }

            private double GetTotal(string statsBasedOn)
            {
                double result = 0;
                switch (statsBasedOn)
                {
                    case "charged":
                        result = Activities.Sum(x => x.ChargeDuration);
                        break;
                    case "scheduled":
                        result = Activities.Sum(x => x.SchedDuration);
                        break;
                    case "actual":
                        result = Activities.Sum(x => x.ActDuration);
                        break;
                }
                return result;
            }

            public string[] ToArray(string statsBasedOn)
            {
                List<string> list = new List<string>
                {
                    ResourceID.ToString(),
                    ProcessTechID.ToString(),
                    LabID.ToString(),
                    ResourceName,
                    ProcessTechName
                };

                switch (statsBasedOn)
                {
                    case "charged":
                        list.AddRange(Activities.Select(x => x.ChargeDuration.ToString("#0.000")).ToArray());
                        break;
                    case "scheduled":
                        list.AddRange(Activities.Select(x => x.SchedDuration.ToString("#0.000")).ToArray());
                        break;
                    case "actual":
                        list.AddRange(Activities.Select(x => x.ActDuration.ToString("#0.000")).ToArray());
                        break;
                }

                list.Add(GetTotal(statsBasedOn).ToString("#0.000"));

                return list.ToArray();
            }
        }

        public class ActivityData
        {
            public int ActivityID { get; set; }
            public string ActivityName { get; set; }
            public double ChargeDuration { get; set; }
            public double SchedDuration { get; set; }
            public double ActDuration { get; set; }
        }

        public class ToolUtilizationItem
        {
            public int ActivityID { get; set; }
            public int ResourceID { get; set; }
            public int ProcessTechID { get; set; }
            public int LabID { get; set; }
            public string ActivityName { get; set; }
            public string ResourceName { get; set; }
            public string ProcessTechName { get; set; }
            public double ChargeDuration { get; set; }
            public double SchedDuration { get; set; }
            public double ActDuration { get; set; }

            public ToolUtilizationItem(DataRow dr)
            {
                ActivityID = Utility.ConvertTo(dr["ActivityID"], 0);
                ResourceID = Utility.ConvertTo(dr["ResourceID"], 0);
                ProcessTechID = Utility.ConvertTo(dr["ProcessTechID"], 0);
                LabID = Utility.ConvertTo(dr["LabID"], 0);
                ActivityName = Utility.ConvertTo(dr["ActivityName"], string.Empty);
                ResourceName = Utility.ConvertTo(dr["ResourceName"], string.Empty);
                ProcessTechName = Utility.ConvertTo(dr["ProcessTechName"], string.Empty);
                ChargeDuration = Utility.ConvertTo(dr["ChargeDuration"], 0D);
                SchedDuration = Utility.ConvertTo(dr["SchedDuration"], 0D);
                ActDuration = Utility.ConvertTo(dr["ActDuration"], 0D);
            }
        }
    }
}

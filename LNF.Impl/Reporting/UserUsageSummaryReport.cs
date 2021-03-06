﻿using LNF.Reporting;
using System.Text;

namespace LNF.Impl.Reporting
{
    public class UserUsageSummaryReport : DefaultReport<UserCriteria>
    {
        public override string Key { get { return "user-usage-summary"; } }
        public override string Title { get { return "User Usage Summary"; } }
        public override string CategoryName { get { return "Individual User Reports"; } }

        public override void WriteCriteria(StringBuilder sb)
        {
            Criteria.CreateWriter(sb)
                .WriteBeginTag("div")
                .WriteText("Period: ")
                .WriteMonthSelect()
                .WriteYearSelect()
                .WriteEndTag()
                .WriteBeginTag("div", new { @style = "margin-top:10px;" })
                .WriteButton("runbutton", "Run Report")
                .WriteEndTag();
        }

        public override GenericResult Execute(ResultType resultType)
        {
            GenericResult result = new GenericResult();
            switch (resultType)
            {
                case ResultType.HTML:
                    break;
            }
            return result;
        }
    }
}

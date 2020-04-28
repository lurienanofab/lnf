using LNF.Impl.Repository.Scheduler;
using LNF.Reporting;
using NHibernate;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Impl.Reporting
{
    public class ToolUtilizationDetail : DefaultReport<ResourceCriteria>
    {
        protected ISession Session { get; }

        public ToolUtilizationDetail(ISession session)
        {
            Session = session;
        }

        public override void WriteCriteria(StringBuilder sb)
        {
            Criteria.CreateWriter(sb)
                .WriteBeginTag("div")
                .WriteSelect("ResourceID", GetResourceSelectItems(), new { @class = "resource-id-select" })
                .WriteEndTag();
        }

        public override string Key
        {
            get { return "tool-utilization-detail"; }
        }

        public override string Title
        {
            get { return "Tool Utilization Detail"; }
        }

        public override string CategoryName
        {
            get { return "Resource Reports"; }
        }

        public override GenericResult Execute(ResultType requestType)
        {
            GenericResult result = new GenericResult();

            return result;
        }

        public IList<GenericListItem> GetResourceSelectItems()
        {
            IList<Resource> query = Session.Query<Resource>().Where(x => x.IsActive).ToList();
            List<GenericListItem> result = new List<GenericListItem>();
            result.AddRange(query
                .OrderBy(x => x.ResourceName)
                .Select(x => new GenericListItem(x.ResourceID, x.ResourceName, x.ResourceID == Criteria.ResourceID)));
            return result;
        }
    }
}

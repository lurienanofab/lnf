using LNF.Impl.Repository.Scheduler;
using LNF.Mail;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Impl.Mail.Criteria
{
    public class ByTool : CriteriaBase
    {
        public int[] SelectedResourceIDs { get; set; }

        public override IEnumerable<MassEmailRecipient> GetRecipients()
        {
            var mgr = new GroupEmailManager(Session);
            return mgr.GetEmailListByTools(SelectedResourceIDs);
        }

        public override string GetGroupName()
        {
            return string.Join(", ", Session.Query<Resource>().Where(x => SelectedResourceIDs.Contains(x.ResourceID)).ToList().Select(x => x.ResourceName));
        }
    }
}

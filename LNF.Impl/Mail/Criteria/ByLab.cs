using LNF.Mail;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Impl.Mail.Criteria
{
    public class ByLab : CriteriaBase
    {
        public int[] SelectedLabs { get; set; }

        public override IEnumerable<MassEmailRecipient> GetRecipients()
        {
            var mgr = new GroupEmailManager(Session);
            return mgr.GetEmailListByInLab(SelectedLabs);
        }

        public override string GetGroupName()
        {
            return string.Join(", ", Provider.PhysicalAccess.GetAreas(SelectedLabs).Select(x => x.Name));
        }
    }
}

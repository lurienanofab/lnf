using LNF.Mail;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Impl.Mail.Criteria
{
    public class ByLab : CriteriaBase
    {
        public int[] SelectedLabs { get; set; }

        protected override IEnumerable<MassEmailRecipient> GetRecipients()
        {
            var mgr = new GroupEmailManager(Session);
            return mgr.GetEmailListByInLab(SelectedLabs);
        }

        protected override string GetGroupName()
        {
            return string.Join(", ", base.Provider.PhysicalAccess.GetAreas(SelectedLabs).Select(x => x.Name));
        }
    }
}

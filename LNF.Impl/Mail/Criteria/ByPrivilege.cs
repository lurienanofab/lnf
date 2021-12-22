using LNF.Data;
using LNF.Mail;
using System.Collections.Generic;

namespace LNF.Impl.Mail.Criteria
{
    public class ByPrivilege : CriteriaBase
    {
        public int SelectedPrivileges { get; set; }

        protected override IEnumerable<MassEmailRecipient> GetRecipients()
        {
            var mgr = new GroupEmailManager(Session);
            return mgr.GetEmailListByPrivilege(SelectedPrivileges);
        }

        protected override string GetGroupName()
        {
            return string.Join(", ", PrivUtility.GetPrivTypes((ClientPrivilege)SelectedPrivileges));
        }
    }
}

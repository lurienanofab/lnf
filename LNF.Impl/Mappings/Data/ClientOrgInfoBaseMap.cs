using LNF.Impl.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal abstract class ClientOrgInfoBaseMap<T> : OrgInfoBaseMap<T> where T : ClientOrgInfoBase
    {
        internal ClientOrgInfoBaseMap()
        {
            MapClientID();
            MapClientOrgID();
            Map(x => x.UserName);
            Map(x => x.FName);
            Map(x => x.MName);
            Map(x => x.LName);
            Map(x => x.DisplayName);
            Map(x => x.Privs);
            Map(x => x.Communities);
            Map(x => x.IsChecked);
            Map(x => x.IsSafetyTest);
            Map(x => x.RequirePasswordReset);
            Map(x => x.ClientActive);
            Map(x => x.TechnicalInterestID);
            Map(x => x.TechnicalInterestName);
            Map(x => x.Phone);
            Map(x => x.Email);
            Map(x => x.IsManager);
            Map(x => x.IsFinManager);
            Map(x => x.SubsidyStartDate);
            Map(x => x.NewFacultyStartDate);
            Map(x => x.ClientAddressID);
            Map(x => x.ClientOrgActive);
            Map(x => x.DepartmentID);
            Map(x => x.DepartmentName);
            Map(x => x.RoleID);
            Map(x => x.RoleName);
            Map(x => x.MaxChargeTypeID);
            Map(x => x.MaxChargeTypeName);
            Map(x => x.EmailRank);
        }

        protected override void MapOrgID()
        {
            Map(x => x.OrgID);
        }

        protected abstract void MapClientID();

        protected abstract void MapClientOrgID();
    }
}

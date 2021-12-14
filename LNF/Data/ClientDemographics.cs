using LNF.DataAccess;

namespace LNF.Data
{
    public class ClientDemographics : IPrivileged, IDataItem
    {
        public virtual int ClientID { get; set; }
        public virtual string UserName { get; set; }
        public virtual string LName { get; set; }
        public virtual string MName { get; set; }
        public virtual string FName { get; set; }
        public virtual string DisplayName => Clients.GetDisplayName(LName, FName);
        public virtual ClientPrivilege Privs { get; set; }
        public virtual bool Active { get; set; }
        public virtual int DemCitizenID { get; set; }
        public virtual string DemCitizenValue { get; set; }
        public virtual int DemGenderID { get; set; }
        public virtual string DemGenderValue { get; set; }
        public virtual int DemRaceID { get; set; }
        public virtual string DemRaceValue { get; set; }
        public virtual int DemEthnicID { get; set; }
        public virtual string DemEthnicValue { get; set; }
        public virtual int DemDisabilityID { get; set; }
        public virtual string DemDisabilityValue { get; set; }
    }
}

using LNF.Data;
using System;

namespace LNF.Impl.Repository.Data
{
    /// <summary>
    /// A base class for subclasses that share common Client data
    /// </summary>
    public abstract class ClientOrgInfoBase : OrgInfoBase, IClient
    {
        /// <summary>
        /// The unique id of a Client
        /// </summary>
        public virtual int ClientID { get; set; }

        /// <summary>
        /// The unique username of a Client used to log in
        /// </summary>
        public virtual string UserName { get; set; }

        /// <summary>
        /// The first name of a Client
        /// </summary>
        public virtual string FName { get; set; }

        /// <summary>
        /// The middle name of a Client
        /// </summary>
        public virtual string MName { get; set; }

        /// <summary>
        /// The last name of a Client
        /// </summary>
        public virtual string LName { get; set; }

        /// <summary>
        /// The display name of a Client
        /// </summary>
        public virtual string DisplayName { get; set; }

        /// <summary>
        /// The privilges granted to a Client
        /// </summary>
        public virtual ClientPrivilege Privs { get; set; }

        /// <summary>
        /// The communities assigned to a Client
        /// </summary>
        public virtual int Communities { get; set; }

        /// <summary>
        /// Indicates if a Client has viewed the new user ethics video
        /// </summary>
        public virtual bool IsChecked { get; set; }

        /// <summary>
        /// Indicates if a Client has passed the new user safety test
        /// </summary>
        public virtual bool IsSafetyTest { get; set; }

        /// <summary>
        /// Indicates if a Client must reset their password on next login
        /// </summary>
        public virtual bool RequirePasswordReset { get; set; }

        /// <summary>
        /// Indicates if a Client is currently active
        /// </summary>
        public virtual bool ClientActive { get; set; }

        /// <summary>
        /// Id used to indicate citizenship demographic data
        /// </summary>
        public virtual int DemCitizenID { get; set; }

        /// <summary>
        /// The assigned citizenship demographic value
        /// </summary>
        public virtual string DemCitizenName { get; set; }

        /// <summary>
        /// Id used to indicate gender demographic data
        /// </summary>
        public virtual int DemGenderID { get; set; }

        /// <summary>
        /// The assigned gender demographic value
        /// </summary>
        public virtual string DemGenderName { get; set; }

        /// <summary>
        /// Id used to indicate racial demographic data
        /// </summary>
        public virtual int DemRaceID { get; set; }

        /// <summary>
        /// The assigned racial demographic value
        /// </summary>
        public virtual string DemRaceName { get; set; }

        /// <summary>
        /// Id used to indicate ethnicity demographic data
        /// </summary>
        public virtual int DemEthnicID { get; set; }

        /// <summary>
        /// The assigned ethnicity demographic value
        /// </summary>
        public virtual string DemEthnicName { get; set; }

        /// <summary>
        /// Id used to indicate disability demographic data
        /// </summary>
        public virtual int DemDisabilityID { get; set; }

        /// <summary>
        /// The assigned disability demographic value
        /// </summary>
        public virtual string DemDisabilityName { get; set; }

        /// <summary>
        /// Id used to indicate a technical interest to which a Client is associated
        /// </summary>
        public virtual int TechnicalInterestID { get; set; }

        /// <summary>
        /// The assigned technical interest name
        /// </summary>
        public virtual string TechnicalInterestName { get; set; }

        /// <summary>
        /// The unique id of a ClientOrg
        /// </summary>
        public virtual int ClientOrgID { get; set; }

        /// <summary>
        /// The phone number of a ClientOrg
        /// </summary>
        public virtual string Phone { get; set; }

        /// <summary>
        /// The email address of a ClientOrg
        /// </summary>
        public virtual string Email { get; set; }

        /// <summary>
        /// Indicates if a ClientOrg is a manager (i.e. a PI)
        /// </summary>
        public virtual bool IsManager { get; set; }

        /// <summary>
        /// Indicates if a ClientOrg is a financial manager
        /// </summary>
        public virtual bool IsFinManager { get; set; }

        /// <summary>
        /// The date when the new user subsidy began
        /// </summary>
        public virtual DateTime? SubsidyStartDate { get; set; }

        /// <summary>
        /// The date when the new faculty subsidy began
        /// </summary>
        public virtual DateTime? NewFacultyStartDate { get; set; }

        /// <summary>
        /// Id of the client address
        /// </summary>
        public virtual int ClientAddressID { get; set; }

        /// <summary>
        /// Indictes if a ClientOrg is currently active
        /// </summary>
        public virtual bool ClientOrgActive { get; set; }

        /// <summary>
        /// Id used to indicate a department to which a ClientOrg is associated
        /// </summary>
        public virtual int DepartmentID { get; set; }

        /// <summary>
        /// The assigned department name
        /// </summary>
        public virtual string DepartmentName { get; set; }

        /// <summary>
        /// Id used to indicate a role to which a ClientOrg is associated
        /// </summary>
        public virtual int RoleID { get; set; }

        /// <summary>
        /// The assigned role name
        /// </summary>
        public virtual string RoleName { get; set; }

        /// <summary>
        /// The maximum ChargeTypeID assigned to a Client - indicates the highest fees that can be charged to a Client
        /// </summary>
        public virtual int MaxChargeTypeID { get; set; }

        /// <summary>
        /// The name that corresponds to the MaxChargeTypeID
        /// </summary>
        public virtual string MaxChargeTypeName { get; set; }

        /// <summary>
        /// The email rank assigned to a ClientOrgInfo - used to determine priority when a Client is assigned to multiple ClientOrgs
        /// </summary>
        public virtual long EmailRank { get; set; }

        public virtual bool IsStaff() => this.HasPriv(ClientPrivilege.Staff);
    }
}

using LNF.Data;
using LNF.Models.Data;
using System.Linq;

namespace LNF.Repository.Data
{
    /// <summary>
    /// Represents a user of the system.
    /// </summary>
    public class Client : ActiveDataItem, IPrivileged
    {
        /// <summary>
        /// The unique id of a Client
        /// </summary>
        public virtual int ClientID { get; set; }

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
        /// The unique username of a Client used to log in
        /// </summary>
        public virtual string UserName { get; set; }

        /// <summary>
        /// Id used to indicate citizenship demographic data
        /// </summary>
        public virtual int DemCitizenID { get; set; }

        /// <summary>
        /// Id used to indicate gender demographic data
        /// </summary>
        public virtual int DemGenderID { get; set; }

        /// <summary>
        /// Id used to indicate racial demographic data
        /// </summary>
        public virtual int DemRaceID { get; set; }

        /// <summary>
        /// Id used to indicate ethnicity demographic data
        /// </summary>
        public virtual int DemEthnicID { get; set; }

        /// <summary>
        /// Id used to indicate disability demographic data
        /// </summary>
        public virtual int DemDisabilityID { get; set; }

        /// <summary>
        /// The privilges granted to a Client
        /// </summary>
        public virtual ClientPrivilege Privs { get; set; }

        /// <summary>
        /// The communities assigned to a Client
        /// </summary>
        public virtual int Communities { get; set; }

        /// <summary>
        /// Id used to indicate a technical field to which a Client is associated
        /// </summary>
        public virtual int TechnicalFieldID { get; set; }

        /// <summary>
        /// Indicates if a Client has viewed the new user ethics video
        /// </summary>
        public virtual bool? IsChecked { get; set; }

        /// <summary>
        /// Indicates if a Client has passed the new user safety test
        /// </summary>
        public virtual bool? IsSafetyTest { get; set; }

        /// <summary>
        /// Indicates if a Client is currently active
        /// </summary>
        public override bool Active { get; set; }

        /// <summary>
        /// The display name of a Client
        /// </summary>
        public virtual string DisplayName
        {
            get { return GetDisplayName(LName, FName); }
        }

        /// <summary>
        /// Gets the record id used in the ActiveLog
        /// </summary>
        /// <returns>A ClientID integer value</returns>
        public override int Record() { return ClientID; }

        /// <summary>
        /// The table name used in the ActiveLog
        /// </summary>
        /// <returns>A table name string value</returns>
        public override string TableName() { return "Client"; }

        /// <summary>
        /// Gets the non null value of IsChecked
        /// </summary>
        /// <returns>True if a Client has watched the ethical video, otherwise false</returns>
        public virtual bool HasWatchedEthicalVideo()
        {
            return IsChecked.GetValueOrDefault();
        }

        /// <summary>
        /// Gets the non null value of IsSafetyTest
        /// </summary>
        /// <returns>True if a Client has completed the safety test, otherwise false</returns>
        public virtual bool HasTakenSafetyTest()
        {
            return IsSafetyTest.GetValueOrDefault();
        }

        /// <summary>
        /// Gets a Client display name in the standard format
        /// </summary>
        public static string GetDisplayName(string lname, string fname)
        {
            return string.Join(", ", new[] { lname, fname }.Where(x => !string.IsNullOrEmpty(x))).Trim();
        }

        public virtual void Update(ClientModel model)
        {
            FName = model.FName;
            MName = model.MName;
            LName = model.LName;
            UserName = model.UserName;
            DemCitizenID = model.DemCitizenID;
            DemGenderID = model.DemGenderID;
            DemRaceID = model.DemRaceID;
            DemEthnicID = model.DemEthnicID;
            DemDisabilityID = model.DemDisabilityID;
            Privs = model.Privs;
            Communities = model.Communities;
            TechnicalFieldID = model.TechnicalFieldID;
            IsChecked = model.IsChecked;
            IsSafetyTest = model.IsSafetyTest;

            if (model.ClientActive)
                this.Enable();
            else
                this.Disable();
        }
    }
}

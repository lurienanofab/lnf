using LNF.Models.Billing;
using System;

namespace LNF.Repository.Billing
{
    /// <summary>
    /// Represents a regular exception Room or Tool billing charge - a charge where the Account is not associated with the Client
    /// </summary>
    public class RegularException : IDataItem
    {
        /// <summary>
        /// The unique id from the original source table (ToolBilling, RoomBilling, etc)
        /// </summary>
        public virtual int BillingID { get; set; }

        /// <summary>
        /// The period this charge occurred
        /// </summary>
        public virtual DateTime Period { get; set; }

        /// <summary>
        /// The billing category - indicates what this charge is for
        /// </summary>
        public virtual BillingCategory BillingCategory { get; set; }

        /// <summary>
        /// The unique id of a Reservation
        /// </summary>
        public virtual int ReservationID { get; set; }

        /// <summary>
        /// The unique id of the Client
        /// </summary>
        public virtual int ClientID { get; set; }

        /// <summary>
        /// The last name of the Client
        /// </summary>
        public virtual string LName { get; set; }

        /// <summary>
        /// The first name of the Client
        /// </summary>
        public virtual string FName { get; set; }

        /// <summary>
        /// The unique id of the invitee Client
        /// </summary>
        public virtual int InviteeClientID { get; set; }

        /// <summary>
        /// The last name of the invitee
        /// </summary>
        public virtual string InviteeLName { get; set; }

        /// <summary>
        /// The first name of the invitee
        /// </summary>
        public virtual string InviteeFName { get; set; }

        /// <summary>
        /// The unique id of a Resource
        /// </summary>
        public virtual int ResourceID { get; set; }

        /// <summary>
        /// The name of the Resource
        /// </summary>
        public virtual string ResourceName { get; set; }

        /// <summary>
        /// The unique id of an Account
        /// </summary>
        public virtual int AccountID { get; set; }

        /// <summary>
        /// The name of an Account
        /// </summary>
        public virtual string AccountName { get; set; }

        /// <summary>
        /// The Account ShortCode
        /// </summary>
        public virtual string ShortCode { get; set; }

        /// <summary>
        /// Indicates if the source table is the billing temp table (current period) or not (previous periods)
        /// </summary>
        public virtual bool IsTemp { get; set; }

        /// <summary>
        /// Determines whether the specified object is equal to the current object - needed for composite key
        /// </summary>
        /// <param name="obj">The object to compare with the current object</param>
        /// <returns>True if the specified object is equal to the current object, otherwise false</returns>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            var x = obj as RegularException;

            if (x == null) return false;

            if (x.BillingID == BillingID && x.BillingCategory == BillingCategory && x.IsTemp == IsTemp)
                return true;

            return false;
        }

        /// <summary>
        /// Serves as the default hash function - needed for composite key
        /// </summary>
        /// <returns>A hash code for the current object</returns>
        public override int GetHashCode()
        {
            return new { BillingID, BillingCategory, IsTemp }.GetHashCode();
        }
    }
}

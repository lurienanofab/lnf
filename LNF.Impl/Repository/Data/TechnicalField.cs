using LNF.DataAccess;

namespace LNF.Impl.Repository.Data
{
    /// <summary>
    /// The technical interest of a Client or technical field of an Account
    /// </summary>
    public class TechnicalField : IDataItem
    {
        /// <summary>
        /// The unique id of a technical field
        /// </summary>
        public virtual int TechnicalFieldID { get; set; }

        /// <summary>
        /// The name of a technical field
        /// </summary>
        public virtual string TechnicalFieldName { get; set; }
    }
}

using LNF.Impl.Repository;
using NHibernate;
using System.Data;

namespace LNF.Impl.Mail
{
    public class InvalidEmailManager
    {
        protected ISession Session { get; }

        public InvalidEmailManager(ISession session)
        {
            Session = session;
        }

        public DataTable GetAllInvalidEmailAddresses()
        {
            return Session.Command()
                .Param("Action", "SelectAll")
                .FillDataTable("dbo.InvalidEmailList_Select");
        }

        public DataTable GetInvalidEmailListFiltering()
        {
            var dt = Session.Command()
                .Param("Action", "SelectFiltering")
                .FillDataTable("dbo.InvalidEmailList_Select");

            dt.PrimaryKey = new[] { dt.Columns["EmailID"] };

            return dt;
        }
    }
}
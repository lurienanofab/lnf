using LNF.CommonTools;
using LNF.Repository;
using System.Data;

namespace LNF.Email
{
    public class InvalidEmailManager
    {
        public static DataTable GetAllInvalidEmailAddresses()
        {
            using (var dba = DA.Current.GetAdapter())
            {
                dba.AddParameter("@Action", "SelectAll");
                return dba.FillDataTable("InvalidEmailList_Select");
            }
        }

        public static DataTable GetInvalidEmailListFiltering()
        {
            using (var dba = DA.Current.GetAdapter())
            {
                dba.AddParameter("@Action", "SelectFiltering");
                DataTable dt = dba.FillDataTable("InvalidEmailList_Select");

                dt.PrimaryKey = new[] { dt.Columns["EmailID"] };

                return dt;
            }
        }
    }
}
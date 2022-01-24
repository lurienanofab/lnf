using LNF.CommonTools;
using NHibernate;
using System;
using System.Data;
using System.Data.SqlClient;

namespace LNF.Impl
{
    /// <summary>
    /// This is used to put exception message into table ExceptionDump.
    /// </summary>
    public class ExceptionManager
    {
        private string _ExpName = string.Empty;
        private string _AppName = string.Empty;
        private string _FunctionName = string.Empty;
        private string _ExceptionMessage = string.Empty;
        private string _CustomData = string.Empty;
        private string _Comment = string.Empty;

        public DateTime TimeStamp { get; set; }

        public string ExpName
        {
            get { return _ExpName; }
            set { _ExpName = Utility.Clip(value, 50); }
        }

        public string AppName
        {
            get { return _AppName; }
            set { _AppName = Utility.Clip(value, 50); }
        }

        public string FunctionName
        {
            get { return _FunctionName; }
            set { _FunctionName = Utility.Clip(value, 50); }
        }

        public string ExceptionMessage
        {
            get { return _ExceptionMessage; }
            set { _ExceptionMessage = Utility.Clip(value, 1000); }
        }

        public string CustomData
        {
            get { return _CustomData; }
            set { _CustomData = Utility.Clip(value, 1000); }
        }

        public string Comment
        {
            get { return _Comment; }
            set { _Comment = Utility.Clip(value, 50); }
        }

        public void LogException(ISession session)
        {
            session.CreateSQLQuery("EXEC dbo.ExceptionDump_Insert @TimeStamp = :TimeStamp, @ExpName = :ExpName, @AppName = :AppName, @FunctionName = :FunctionName, @ExceptionMessage = :ExceptionMessage, @CustomData = :CustomData, @Comment = :Comment")
                .SetParameter("TimeStamp", TimeStamp)
                .SetParameter("ExpName", ExpName)
                .SetParameter("AppName", AppName)
                .SetParameter("FunctionName", FunctionName)
                .SetParameter("ExceptionMessage", ExceptionMessage)
                .SetParameter("CustomData", CustomData)
                .SetParameter("Comment", Comment)
                .ExecuteUpdate();
        }

        public void LogException(SqlConnection conn)
        {
            using (var cmd = conn.CreateCommand("dbo.ExceptionDump_Insert"))
            {
                cmd.Parameters.AddWithValue("TimeStamp", TimeStamp);
                cmd.Parameters.AddWithValue("ExpName", ExpName);
                cmd.Parameters.AddWithValue("AppName", AppName);
                cmd.Parameters.AddWithValue("FunctionName", FunctionName);
                cmd.Parameters.AddWithValue("ExceptionMessage", ExceptionMessage);
                cmd.Parameters.AddWithValue("CustomData", CustomData);
                cmd.Parameters.AddWithValue("Comment", Comment);
                cmd.ExecuteNonQuery();
            }
        }

        public void Purge(ISession session) => session.CreateSQLQuery("dbo.ExceptionDump_Delete").ExecuteUpdate();

        public void Purge(SqlConnection conn)
        {
            using (var cmd = conn.CreateCommand("dbo.ExceptionDump_Delete"))
            {
                cmd.ExecuteNonQuery();
            }
        }
    }
}

using LNF.Repository;
using System;

namespace LNF.CommonTools
{
    /// <summary>
    /// This is used to put exception message into table ExceptionDump.
    /// </summary>
    public class ExceptionManager
    {
        public DateTime TimeStamp;
        private string _ExpName = string.Empty;
        private string _AppName = string.Empty;
        private string _FunctionName = string.Empty;
        private string _ExceptionMessage = string.Empty;
        private string _CustomData = string.Empty;
        private string _Comment = string.Empty;

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

        public void LogException()
        {
            InsertIntoDB();
        }

        private void InsertIntoDB()
        {
            DA.Command()
                .Param(new { TimeStamp, ExpName, AppName, FunctionName, ExceptionMessage, CustomData, Comment })
                .ExecuteNonQuery("dbo.ExceptionDump_Insert");
        }

        public static void Purge() => DA.Command().ExecuteNonQuery("dbo.ExceptionDump_Delete");
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository;

namespace LNF.CommonTools
{
    /// <summary>
    /// This is used to put exceptional message into table ExceptionDump
    /// </summary>
    /// <remarks>
    /// Usage Scenario
    /// ExceptionManager exp = new ExceptionManager {TimeStamp = DateTime.Now, ExpName = "User has zero or negagive TotalOriginalPayment", AppName = this.GetType().Assembly.GetName().Name, FunctionName = "CommonTools-DistributeSubsidyMoneyEvenly"};
    /// exp.CustomData = String.Format("ClientID = {0}, Period = '{1}'", ClientID, Period);
    /// exp.LogException();
    ///</remarks>
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
            get
            {
                return _ExpName;
            }
            set
            {
                if (value.Length > 50)
                    _ExpName = value.Substring(0, 50);
                else
                    _ExpName = value;
            }
        }

        public string AppName
        {
            get
            {
                return _AppName;
            }
            set
            {
                if (value.Length > 50)
                    _AppName = value.Substring(0, 50);
                else
                    _AppName = value;
            }
        }

        public string FunctionName
        {
            get
            {
                return _FunctionName;
            }
            set
            {
                if (value.Length > 50)
                    _FunctionName = value.Substring(0, 50);
                else
                    _FunctionName = value;
            }
        }

        public string ExceptionMessage
        {
            get
            {
                return _ExceptionMessage;
            }
            set
            {
                if (value.Length > 1000)
                    _ExceptionMessage = value.Substring(0, 1000);
                else
                    _ExceptionMessage = value;
            }
        }

        public string CustomData
        {
            get
            {
                return _CustomData;
            }
            set
            {
                if (value.Length > 1000)
                    _CustomData = value.Substring(0, 1000);
                else
                    _CustomData = value;
            }
        }

        public string Comment
        {
            get
            {
                return _Comment;
            }
            set
            {
                if (value.Length > 50)
                    _Comment = value.Substring(0, 50);
                else
                    _Comment = value;
            }
        }

        public void LogException()
        {
            InsertIntoDB();
        }

        private void InsertIntoDB()
        {
            using (var dba = DA.Current.GetAdapter())
            {
                dba.SelectCommand.ApplyParameters(new
                    {
                        TimeStamp = TimeStamp,
                        ExpName = _ExpName,
                        AppName = _AppName,
                        FunctionName = _FunctionName,
                        ExceptionMessage = _ExceptionMessage,
                        CustomData = _CustomData,
                        Comment = _Comment
                    }).ExecuteNonQuery("ExceptionDump_Insert");
            }
        }

        public static void CleanOldException()
        {
            using (var dba = DA.Current.GetAdapter())
                dba.SelectCommand.ExecuteNonQuery("ExceptionDump_Delete");
        }
    }
}

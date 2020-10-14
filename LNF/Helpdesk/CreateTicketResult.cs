using System;
using System.Data;

namespace LNF.Helpdesk
{
    public class CreateTicketResult
    {
        public CreateTicketResult() { }

        internal CreateTicketResult(DataTable data)
        {
            Data = data;
            Exception = null;
        }

        internal CreateTicketResult(Exception ex)
        {
            Data = null;
            Exception = ex;
        }

        internal CreateTicketResult(DataTable data, Exception ex)
        {
            Data = data;
            Exception = ex;
        }

        public bool Success { get { return Exception == null; } }
        public int HardwareTicketEmailsSent { get; set; }
        public DataTable Data { get; }
        public Exception Exception { get; }
    }
}

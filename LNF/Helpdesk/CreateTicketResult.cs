using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace LNF.Helpdesk
{
    public class CreateTicketResult
    {
        private DataTable _Data;
        private Exception _Exception;

        public CreateTicketResult() { }

        internal CreateTicketResult(DataTable data)
        {
            _Data = data;
            _Exception = null;
        }

        internal CreateTicketResult(Exception ex)
        {
            _Data = null;
            _Exception = ex;
        }

        internal CreateTicketResult(DataTable data, Exception ex)
        {
            _Data = data;
            _Exception = ex;
        }

        public bool Success { get { return _Exception == null; } }
        public DataTable Data { get { return _Data; } }
        public Exception Exception { get { return _Exception; } }
    }
}

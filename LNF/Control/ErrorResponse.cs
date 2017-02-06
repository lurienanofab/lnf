using System;

namespace LNF.Control
{
    public class ErrorResponse : ControlResponse
    {
        public Exception Exception { get; }

        public ErrorResponse(Exception ex)
        {
            Exception = ex;
            Error = true;
            Message = ex.Message;
        }
    }
}

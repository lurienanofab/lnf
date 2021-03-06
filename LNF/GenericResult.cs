﻿using LNF.Logging;
using System;
using System.Collections.Generic;

namespace LNF
{
    public class GenericResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public IEnumerable<LogMessage> Log { get; set; }
        public dynamic Data { get; set; }
        
        public GenericResult()
        {
            Success = true;
            Message = string.Empty;
            Data = null;
        }

        public void AppendLine(string text)
        {
            Message += text + Environment.NewLine;
        }
    }
}

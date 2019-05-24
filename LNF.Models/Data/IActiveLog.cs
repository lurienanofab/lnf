﻿using System;

namespace LNF.Models.Data
{
    public interface IActiveLog
    {
        int LogID { get; set; }
        string TableName { get; set; }
        int Record { get; set; }
        DateTime EnableDate { get; set; }
        DateTime? DisableDate { get; set; }
    }
}

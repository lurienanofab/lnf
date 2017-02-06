﻿using LNF.Repository.Data;
using System;

namespace LNF.Repository.Inventory
{
    public class ChemicalLabelPrintLog : IDataItem
    {
        public virtual int ChemicalLabelPrintLogID { get; set; }
        public virtual ChemicalLocation ChemicalLocation { get; set; }
        public virtual Client Client { get; set; }
        public virtual DateTime PrintDateTime { get; set; }
        public virtual string IPAddress { get; set; }
    }
}

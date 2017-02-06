﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Data;
using LNF.Repository.Data;
using LNF.Repository.Inventory;

namespace LNF.Repository.Store
{
    public class StoreOrder : IDataItem
    {
        public virtual int SOID { get; set; }
        public virtual Client Client { get; set; }
        public virtual Account Account { get; set; }
        public virtual DateTime CreationDate { get; set; }
        public virtual string Status { get; set; }
        public virtual DateTime StatusChangeDate { get; set; }
        public virtual int? InventoryLocationID { get; set; }
    }
}

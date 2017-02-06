﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository;
using LNF.Repository.Data;

namespace LNF.Repository.Data
{
    public class MiscBillingCharge : IDataItem
    {
        public virtual int ExpID { get; set; }
        public virtual Client Client { get; set; }
        public virtual Account Account { get; set; }
        public virtual string SUBType { get; set; }
        public virtual DateTime Period { get; set; }
        public virtual DateTime ActDate { get; set; }
        public virtual string Description { get; set; }
        public virtual decimal Quantity { get; set; }
        public virtual decimal UnitCost{ get; set; }
        public virtual decimal SubsidyDiscount { get; set; }
        public virtual bool Active { get; set; }

        /// <summary>
        /// The total charge used to calculate subsidy.
        /// </summary>
        public virtual decimal GetTotalCost()
        {
            // dt.Columns.Add("TotalCost", typeof(double), "Quantity * UnitCost");

            // this matches the stored procedure TieredSubsidyBilling_Select @Action = 'ForSubsidyDiscountDistribution'
            return Quantity * UnitCost;
        }

        public virtual decimal GetUserPayment()
        {
            // dt.Columns.Add("UserPayment", typeof(double), "(Quantity * UnitCost) - SubsidyDiscount");

            return GetTotalCost() - SubsidyDiscount;
        }

        /// <summary>
        /// The final billed amount based on BillingType and Room
        /// </summary>
        public virtual decimal GetLineCost()
        {
            // [2015-11-13 jg] same as TotalCharge - added to be consistent with ToolBilling and RoomBilling
            return GetTotalCost();
        }
    }
}

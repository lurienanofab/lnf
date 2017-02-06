using System;
using System.Collections.Generic;
using System.Text;

namespace LNF.Repository.Data
{
    public class FundingSource : IDataItem
    {
        public FundingSource() { }
        public virtual int FundingSourceID { get; set; }
        public virtual string FundingSourceName { get; set; }
    }
}

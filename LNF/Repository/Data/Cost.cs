using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Repository.Data
{
    public class Cost : IDataItem
    {
        public virtual int CostID { get; set; }
        public virtual ChargeType ChargeType { get; set; }
        public virtual string TableNameOrDescription { get; set; }
        public virtual int RecordID { get; set; }
        public virtual string AcctPer { get; set; }
        public virtual decimal AddVal { get; set; }
        public virtual decimal MulVal { get; set; }
        public virtual DateTime EffDate { get; set; }
        public virtual DateTime CreatedDate { get; set; }

        public static IList<Cost> SelectToolCosts()
        {
            string[] tableName = { "ToolCost", "ToolOvertimeCost" };
            var result = DA.Current.Query<Cost>().Where(x => tableName.Contains(x.TableNameOrDescription)).ToList();
            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Repository.Meter
{
    public class FileImport : IDataItem
    {
        public virtual int FileImportID { get; set; }
        public virtual string ImportFileName { get; set; }
        public virtual DateTime ImportDate { get; set; }
        public virtual string ImportResult { get; set; }
        public virtual int TotalRows { get; set; }
    }
}

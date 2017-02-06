using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LNF.Data.ClientAccountMatrix
{
    public class MatrixAccount
    {
        public int AccountID { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public string Project { get; set; }
        public string ShortCode { get; set; }
    }
}
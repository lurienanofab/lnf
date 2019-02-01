using System;
using System.Linq;

namespace LNF.Models
{
    public static class Utility
    {
        public static string EnumToString(Enum value, string separator = "|")
        {
            // The normal enum ToString() method returns "Value1, Value2, Value3, ..."
            // This method will return "Value1|Value2|Value3..." instead.
            return string.Join(separator, value.ToString().Split(',').Select(x => x.Trim()));
        }
    }
}

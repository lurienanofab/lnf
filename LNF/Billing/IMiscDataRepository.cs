using System;
using System.Data;

namespace LNF.Billing
{
    public interface IMiscDataRepository
    {
        DataTable ReadMiscData(DateTime period);
    }
}
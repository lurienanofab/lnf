using LNF.Repository;
using System;
using System.Data;

namespace LNF.CommonTools
{
    public interface IReadMiscDataManager : IManager
    {
        DataTable ReadMiscData(DateTime period);
    }
}
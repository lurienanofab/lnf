using LNF.Repository;
using System;
using System.Data;

namespace LNF.CommonTools
{
    public enum StoreDataCleanOption
    {
        AllItems = 0,
        RechargeItems = 1
    }

    public interface IReadStoreDataManager : IManager
    {
        DataTable ReadStoreDataRaw(DateTime sd, DateTime ed, int clientId = 0);
        DataTable ReadStoreDataClean(DateTime sd, DateTime ed, int clientId = 0, int itemId = 0, StoreDataCleanOption option = StoreDataCleanOption.AllItems);
        DataTable ReadStoreData(DateTime period, int clientId = 0, int itemId = 0);
    }
}
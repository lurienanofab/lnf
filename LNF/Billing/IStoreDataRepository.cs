using System;
using System.Data;

namespace LNF.Billing
{
    public interface IStoreDataRepository
    {
        DataTable ReadStoreDataRaw(DateTime sd, DateTime ed, int clientId = 0);
        DataTable ReadStoreDataClean(DateTime sd, DateTime ed, int clientId = 0, int itemId = 0, StoreDataCleanOption option = StoreDataCleanOption.AllItems);
        DataTable ReadStoreData(DateTime period, int clientId = 0, int itemId = 0);
        int LoadDryBoxBilling(DateTime sd, DateTime ed);
        int DeleteStoreDataClean(DateTime sd, DateTime ed, int clientId = 0, int itemId = 0, int catId = 0);
    }
}
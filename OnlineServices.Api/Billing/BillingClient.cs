using LNF.Models.Billing;
using LNF.Models.Billing.Process;
using LNF.Models.Billing.Reports;
using LNF.Models.Billing.Reports.ServiceUnitBilling;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

namespace OnlineServices.Api.Billing
{
    public class BillingClient : ApiClient
    {
        public BillingClient() : base(ConfigurationManager.AppSettings["ApiHost"]) { }

        public async Task<ToolSUB> GetToolSUB(DateTime sd, DateTime ed, int clientId = 0)
        {
            return await Get<ToolSUB>(string.Format("billing/report/tool/sub?sd={0:yyyy-MM-dd}&ed={1:yyyy-MM-dd}&clientId={2}", sd, ed, clientId));
        }

        public async Task<RoomSUB> GetRoomSUB(DateTime sd, DateTime ed, int clientId = 0)
        {
            return await Get<RoomSUB>(string.Format("billing/report/room/sub?sd={0:yyyy-MM-dd}&ed={1:yyyy-MM-dd}", sd, ed, clientId));
        }

        public async Task<string> Test()
        {
            return await Get<string>("billing/test");
        }

        public async Task<StoreSUB> GetStoreSUB(DateTime sd, DateTime ed, bool twoCreditAccounts, int clientId)
        {
            var opt = twoCreditAccounts ? "&option=two-credit-accounts" : string.Empty;
            return await Get<StoreSUB>(string.Format("billing/report/store/sub?sd={0:yyyy-MM-dd}&ed={1:yyyy-MM-dd}&clientId={2}{3}", sd, ed, clientId, opt));
        }

        public async Task<ToolJU> GetToolJU(DateTime sd, DateTime ed, JournalUnitTypes type, int clientId)
        {
            return await Get<ToolJU>(string.Format("billing/report/tool/ju/{0}?sd={1:yyyy-MM-dd}&ed={2:yyyy-MM-dd}&clientId={3}", EnumToString(type), sd, ed, clientId));
        }

        public async Task<RoomJU> GetRoomJU(DateTime sd, DateTime ed, JournalUnitTypes type, int clientId)
        {
            return await Get<RoomJU>(string.Format("billing/report/room/ju/{0}?sd={1:yyyy-MM-dd}&ed={2:yyyy-MM-dd}&clientId={3}", EnumToString(type), sd, ed, clientId));
        }

        public async Task<BillingProcessResult> BillingProcessDataClean(BillingCategory billingCategory, DateTime sd, DateTime ed, int clientId, int record)
        {
            BillingProcessDataCommand model = new BillingProcessDataCommand()
            {
                BillingCategory = billingCategory,
                StartPeriod = sd,
                EndPeriod = ed,
                ClientID = clientId,
                Record = record
            };

            return await Post<BillingProcessResult>("billing/process/data/clean", model);
        }

        public async Task<BillingProcessResult> BillingProcessData(BillingCategory billingCategory, DateTime sd, DateTime ed, int clientId, int record)
        {
            BillingProcessDataCommand model = new BillingProcessDataCommand()
            {
                BillingCategory = billingCategory,
                StartPeriod = sd,
                EndPeriod = ed,
                ClientID = clientId,
                Record = record
            };

            return await Post<BillingProcessResult>("billing/process/data", model);
        }

        public async Task<BillingProcessResult> UpdateClientBilling(DateTime period, int clientId)
        {
            UpdateClientBillingCommand model = new UpdateClientBillingCommand () { Period = period, ClientID = clientId };
            return await Post<BillingProcessResult>("billing/update-client", model);
        }

        public async Task<BillingProcessResult> BillingProcessStep1(BillingCategory billingCategory, DateTime sd, DateTime ed, int clientId, int record, bool isTemp, bool delete)
        {
            BillingProcessStep1Command model = new BillingProcessStep1Command()
            {
                BillingCategory = billingCategory,
                StartPeriod = sd,
                EndPeriod = ed,
                ClientID = clientId,
                Record = record,
                Delete = delete,
                IsTemp = isTemp
            };

            return await Post<BillingProcessResult>("billing/process/step1", model);
        }

        public async Task<BillingProcessResult> BillingProcessStep4(string command, DateTime period, int clientId)
        {
            BillingProcessStep4Command model = new BillingProcessStep4Command()
            {
                Period = period,
                ClientID = clientId,
                Command = command
            };

            return await Post<BillingProcessResult>("billing/process/step4", model);
        }

        public async Task<AccountSubsidyModel> DisableAccountSubsidy(int accountSubsidyId)
        {
            await Task.FromResult(0);
            throw new NotImplementedException();
        }

        public async Task<BillingProcessResult> BillingProcessDataFinalize(DateTime sd, DateTime ed)
        {
            BillingProcessDataFinalizeCommand model = new BillingProcessDataFinalizeCommand()
            {
                 StartPeriod = sd,
                 EndPeriod = ed
            };

            return await Post<BillingProcessResult>("billing/process/data/update", model);
        }

        public async Task<BillingProcessResult> BillingProcessDataUpdate(BillingCategory billingCategory, bool isDailyImport)
        {
            BillingProcessDataUpdateCommand model = new BillingProcessDataUpdateCommand()
            {
                BillingCategory = billingCategory,
                IsDailyImport = isDailyImport
            };

            return await Post<BillingProcessResult>("billing/process/data/update", model);
        }

        public async Task<int> SendUserApportionmentReport(UserApportionmentReportOptions model)
        {
            return await Post<int>("billing/report/user-apportionment", model);
        }

        public async Task<int> SendFinancialManagerReport(FinancialManagerReportOptions model)
        {
            return await Post<int>("billing/report/financial-manager", model);
        }

        private string EnumToString(Enum value)
        {
            return value.ToString().ToLower().Replace(" ", string.Empty);
        }

        public async Task<IEnumerable<ToolBillingModel>> GetToolBilling(DateTime period, int clientId)
        {
            return await Get<IEnumerable<ToolBillingModel>>(string.Format("billing/tool?period={0:yyyy-MM-dd}&clientId={1}", period, clientId));
        }

        public async Task<IEnumerable<RoomBillingModel>> GetRoomBilling(DateTime period, int clientId)
        {
            return await Get<IEnumerable<RoomBillingModel>>(string.Format("billing/room?period={0:yyyy-MM-dd}&clientId={1}", period, clientId));
        }
    }
}

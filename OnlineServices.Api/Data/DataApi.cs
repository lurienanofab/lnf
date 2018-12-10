using LNF.Models.Data;

namespace OnlineServices.Api.Data
{
    public class DataApi : IDataApi
    {
        public DataApi(IDefaultClient @default, IClientClient client)
        {
            Default = @default;
            Client = client;
        }

        public IDefaultClient Default { get; }
        public IClientClient Client { get; }
    }
}

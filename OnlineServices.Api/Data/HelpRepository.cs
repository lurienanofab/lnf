using LNF.Data;
using RestSharp;

namespace OnlineServices.Api.Data
{
    public class HelpRepository : ApiClient, IHelpRepository
    {
        internal HelpRepository(IRestClient rc) : base(rc) { }
    }
}

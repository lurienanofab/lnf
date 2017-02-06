using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace OnlineServices.Api.Authorization.Credentials
{
    public interface ICredentials
    {
        HttpContent CreateContent();
    }
}

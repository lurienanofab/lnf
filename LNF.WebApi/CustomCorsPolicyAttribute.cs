using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Cors;
using System.Web.Http.Cors;

namespace LNF.WebApi
{
    public class CustomCorsPolicyAttribute : Attribute, ICorsPolicyProvider
    {
        public Task<CorsPolicy> GetCorsPolicyAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var policy = new CorsPolicy() { AllowAnyMethod = true, AllowAnyHeader = true, SupportsCredentials = true };

            //var username = request.GetRequestContext().Principal.Identity.Name;

            //these defaults are always allowed
            policy.Origins.Add("http://ssel-apps.eecs.umich.edu");
            policy.Origins.Add("https://ssel-apps.eecs.umich.edu");
            policy.Origins.Add("http://ssel-sched.eecs.umich.edu");
            policy.Origins.Add("https://ssel-sched.eecs.umich.edu");
            policy.Origins.Add("http://staging.ssel-sched.eecs.umich.edu");
            policy.Origins.Add("http://staging.ssel-sched.eecs.umich.edu/");
            policy.Origins.Add("http://lnf-dev.eecs.umich.edu");
            policy.Origins.Add("http://lnf-jgett.eecs.umich.edu");
            policy.Origins.Add("http://lnf-pagadala.eecs.umich.edu");
            policy.Origins.Add("http://editor.swagger.io");
            policy.Origins.Add("https://editor.swagger.io");
            policy.Origins.Add("http://online.swagger.io/");
            policy.Origins.Add("https://online.swagger.io/");

            return Task.FromResult(policy);
        }
    }
}

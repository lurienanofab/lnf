using LNF.Impl.DependencyInjection.Default;
using LNF.Repository;
using Moq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Principal;
using System.Web;

namespace LNF.Impl.Testing
{
    public class ContextManager : IDisposable
    {
        private readonly IDisposable _uow;
        private string _redirectLocation;

        public string Username { get; set; }
        public string IPAddress { get; set; }
        public IDictionary ContextItems { get; }
        public SessionItemCollection SessionItems { get; }
        public NameValueCollection QueryString { get; }
        public IOC IOC { get; }
        public HttpContextBase ContextBase { get; }

        public ContextManager(string ipaddr, string username, IDictionary contextItems = null, SessionItemCollection sessionItems = null, NameValueCollection queryString = null)
        {
            if (string.IsNullOrEmpty(ipaddr))
                ipaddr = "127.0.0.1";

            if (contextItems == null)
                contextItems = new Dictionary<object, object>();

            if (sessionItems == null)
                sessionItems = new SessionItemCollection();

            if (queryString == null)
                queryString = new NameValueCollection();

            Username = string.Empty;
            IPAddress = ipaddr;
            ContextItems = contextItems;
            SessionItems = sessionItems;
            QueryString = queryString;

            ContextBase = CreateHttpContext();

            var ctx = new TestContext(ContextBase);

            IOC = new IOC(ctx);
            ServiceProvider.Current = IOC.Resolver.GetInstance<ServiceProvider>();

            Login(username);

            _uow = DA.StartUnitOfWork();
        }

        public HttpContextBase CreateHttpContext()
        {
            var context = new Mock<HttpContextBase>();

            var session = new Mock<HttpSessionStateBase>();
            session.Setup(x => x.Remove(It.IsAny<string>())).Callback<string>(SessionItems.Remove);
            session.Setup(x => x[It.IsAny<string>()]).Returns<string>(SessionItems.Get);
            session.SetupSet(x => x["Cache"] = It.IsAny<object>()).Callback<string, object>(SessionItems.Set);
            ConfigureMockSession(session);

            var request = new Mock<HttpRequestBase>();
            request.Setup(x => x.QueryString).Returns(QueryString);
            request.Setup(x => x.UserHostAddress).Returns(() => IPAddress);
            ConfigureMockRequest(request);

            var response = new Mock<HttpResponseBase>();
            response.Setup(x => x.Redirect(It.IsAny<string>())).Callback<string>(x => _redirectLocation = x);
            response.SetupGet(x => x.RedirectLocation).Returns(() => _redirectLocation);

            var identity = new Mock<IIdentity>();
            identity.Setup(x => x.Name).Returns(() => Username);
            identity.Setup(x => x.IsAuthenticated).Returns(() => !string.IsNullOrEmpty(Username));
            ConfigureMockIdentity(identity);

            var principal = new Mock<IPrincipal>();
            principal.Setup(x => x.Identity).Returns(identity.Object);
            ConfigureMockPrincipal(principal);

            context.Setup(x => x.Items).Returns(ContextItems);
            context.Setup(x => x.Session).Returns(session.Object);
            context.Setup(x => x.Request).Returns(request.Object);
            context.Setup(x => x.Response).Returns(response.Object);
            context.Setup(x => x.User).Returns(principal.Object);
            ConfigureMockContext(context);

            return context.Object;
        }

        /// <summary>
        /// Does nothing unless overridden. Note: the Remove method, common session variable setters, and indexer have already been setup. Override to setup application specific Session variable setters.
        /// </summary>
        public virtual void ConfigureMockSession(Mock<HttpSessionStateBase> session) { }

        /// <summary>
        /// Does nothing unless overridden. Note: the QueryString, and UserHostAddress properties have already been setup.
        /// </summary>
        public virtual void ConfigureMockRequest(Mock<HttpRequestBase> request) { }

        /// <summary>
        /// Does nothing unless overridden.
        /// </summary>
        public virtual void ConfigureMockResponse(Mock<HttpResponseBase> request) { }

        /// <summary>
        /// Does nothing unless overridden. Note: the Name, and IsAuthenticated properties have already been setup.
        /// </summary>
        public virtual void ConfigureMockIdentity(Mock<IIdentity> identity) { }

        /// <summary>
        /// Does nothing unless overridden. Note: the Identity property has already been setup.
        /// </summary>
        public virtual void ConfigureMockPrincipal(Mock<IPrincipal> principal) { }

        /// <summary>
        /// Does nothing unless overridden. Note: the Items, Session, Request, and User properties have already been setup.
        /// </summary>
        public virtual void ConfigureMockContext(Mock<HttpContextBase> context) { }

        public void Login(string username)
        {
            Username = username;
            ContextItems.Remove("CurrentUser");
        }

        public void Dispose()
        {
            if (_uow != null)
                _uow.Dispose();
        }
    }

    public class SessionItemCollection : IEnumerable<KeyValuePair<string, object>>
    {
        private readonly IDictionary<string, object> _items;

        public SessionItemCollection() : this(new Dictionary<string, object>()) { }

        public SessionItemCollection(IDictionary<string, object> items)
        {
            _items = items;
        }

        public object this[string key]
        {
            get => Get(key);
            set => Set(key, value);
        }

        public object Get(string key)
        {
            if (_items.ContainsKey(key))
                return _items[key];
            else
                return null;
        }

        public void Set(string key, object value)
        {
            if (_items.ContainsKey(key))
                _items[key] = value;
            else
                _items.Add(key, value);
        }

        public void Remove(string key)
        {
            if (_items.ContainsKey(key))
                _items.Remove(key);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

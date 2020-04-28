using LNF.Data;
using LNF.DataAccess;
using LNF.Impl.Repository.Data;
using LNF.Impl.Repository.Scheduler;
using LNF.Scheduler;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Impl
{
    public static class ModelFactoryExtensions
    {
        public static NHibernate.ISession Session => ModelFactoryProvider.Current.Factory.Session;

        public static T CreateModel<T>(this IDataItem item)
        {
            return ModelFactoryProvider.Current.Factory.Create<T>(item);
        }

        public static IEnumerable<IClient> CreateModels(this IQueryable<Client> query)
        {
            var join = query.Join(Session.Query<ClientInfo>(), o => o.ClientID, i => i.ClientID, (o, i) => i);
            return CreateModels<IClient>(join);
        }

        public static IEnumerable<IClient> CreateModels(this IQueryable<ClientOrg> query)
        {
            var join = query.Join(Session.Query<ClientOrgInfo>(), o => o.ClientOrgID, i => i.ClientOrgID, (o, i) => i);
            return CreateModels<IClient>(join);
        }

        public static IEnumerable<IClientAccount> CreateModels(this IQueryable<ClientAccount> query)
        {
            var join = query.Join(Session.Query<ClientAccountInfo>(), o => o.ClientAccountID, i => i.ClientAccountID, (o, i) => i);
            return CreateModels<IClientAccount>(join);
        }

        public static IEnumerable<IOrg> CreateModels(this IQueryable<Org> query)
        {
            var join = query.Join(Session.Query<OrgInfo>(), o => o.OrgID, i => i.OrgID, (o, i) => i);
            return CreateModels<IOrg>(join);
        }

        public static IEnumerable<IAccount> CreateModels(this IQueryable<Account> query)
        {
            var join = query.Join(Session.Query<AccountInfo>(), o => o.AccountID, i => i.AccountID, (o, i) => i);
            return CreateModels<IAccount>(join);
        }

        public static IEnumerable<IReservation> CreateModels(this IQueryable<Reservation> query)
        {
            var join = query.Join(Session.Query<ReservationInfo>(), o => o.ReservationID, i => i.ReservationID, (o, i) => i);
            return CreateModels<IReservation>(join);
        }

        public static IEnumerable<IResource> CreateModels(this IQueryable<Resource> query)
        {
            var join = query.Join(Session.Query<ResourceInfo>(), o => o.ResourceID, i => i.ResourceID, (o, i) => i);
            return CreateModels<IResource>(join);
        }

        public static IEnumerable<IResourceClient> CreateModels(this IQueryable<ResourceClient> query)
        {
            var join = query.Join(Session.Query<ResourceClientInfo>(), o => o.ResourceClientID, i => i.ResourceClientID, (o, i) => i);
            return CreateModels<IResourceClient>(join);
        }

        public static IEnumerable<T> CreateModels<T>(this IEnumerable<IDataItem> query)
        {
            var list = query.ToList();
            var result = new List<T>();

            foreach (var item in list)
            {
                var model = item.CreateModel<T>();
                result.Add(model);
            }

            return result;
        }
    }

}

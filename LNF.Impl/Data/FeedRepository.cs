using LNF.Data;
using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using LNF.Impl.Repository.Data;
using LNF.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.Impl.Data
{
    public class FeedRepository : RepositoryBase, IFeedRepository
    {
        public FeedRepository(ISessionManager mgr, IScriptEngine scriptEngine) : base(mgr)
        {
            ScriptEngine = scriptEngine;
        }

        public IScriptEngine ScriptEngine { get; }

        public DataFeedResult GetDataFeedResult(string alias, string key, IDictionary<object, object> parameters = null)
        {
            var feed = Session.Query<DataFeed>().FirstOrDefault(x => x.FeedAlias == alias);

            DataFeedResult result = null;

            if (feed != null)
            {
                result = new DataFeedResult
                {
                    ID = feed.FeedID,
                    GUID = feed.FeedGUID,
                    Alias = feed.FeedAlias,
                    Name = feed.FeedName,
                    Description = feed.FeedDescription,
                    Private = feed.Private,
                    Active = feed.Active,
                    Deleted = feed.Deleted,
                    Data = null
                };

                if (result.Deleted)
                    return result;

                if (parameters == null)
                    parameters = new Dictionary<object, object>();

                feed.ApplyDefaultParameters(parameters);

                DataSet ds = ExecuteQuery(feed, ScriptParameters.Create(parameters));
                result.Name = ds.DataSetName;
                result.Data = new DataFeedResultSet();

                IList<DataTable> tables = GetTables(ds, key);

                foreach (DataTable dt in tables)
                {
                    var items = new DataFeedResultItemCollection();
                    foreach (DataRow dr in dt.Rows)
                    {
                        var item = new DataFeedResultItem();
                        foreach (DataColumn dc in dt.Columns)
                        {
                            item[dc.ColumnName] = dr[dc.ColumnName].ToString();
                        }
                        items.Add(item);
                    }

                    result.Data[dt.TableName] = items;
                }
            }

            return result;
        }

        private DataSet ExecuteQuery(DataFeed feed, ScriptParameters parameters)
        {
            DataSet ds = new DataSet();
            DataTable dt = null;

            string sql = feed.FeedQuery;
            string name = feed.FeedName;

            if (parameters != null)
                name = parameters.Replace(name);

            ds.DataSetName = name;

            if (feed.FeedType == DataFeedType.SQL)
            {
                var command = ReadOnlyDataCommand.Create(CommandType.Text);

                if (parameters != null)
                {
                    sql = parameters.Replace(sql);
                    command.Param(parameters);
                }

                command.FillDataSet(ds, sql);

                if (ds.Tables.Count > 0)
                    ds.Tables[0].TableName = "default";
            }
            else
            {
                ScriptResult result = ScriptEngine.Run(feed.FeedQuery, parameters);
                if (result.Exception != null)
                    throw result.Exception;
                foreach (var kvp in result.DataSet)
                {
                    dt = kvp.Value.AsDataTable();
                    dt.TableName = kvp.Key;
                    ds.Tables.Add(dt);
                }
            }

            return ds;
        }

        private static IList<DataTable> GetTables(DataSet ds, string key)
        {
            IList<DataTable> result = null;
            if (string.IsNullOrEmpty(key))
                result = ds.Tables.Cast<DataTable>().ToList();
            else
                result = ds.Tables.Cast<DataTable>().Where(x => x.TableName == key).ToList();
            return result;
        }

        public IDataFeed GetDataFeed(int feedId)
        {
            return Session.Get<DataFeed>(feedId).CreateModel<IDataFeed>();
        }

        public IDataFeed GetDataFeed(string alias)
        {
            return Session.Query<DataFeed>().FirstOrDefault(x => x.FeedAlias == alias).CreateModel<IDataFeed>();
        }

        public IFeedsLog AddFeedsLogEntry(string requestIp, string requestUrl, string userAgent)
        {
            FeedsLog log = new FeedsLog
            {
                EntryDateTime = DateTime.Now,
                RequestIP = requestIp,
                RequestURL = requestUrl,
                RequestUserAgent = userAgent ?? "unknown"
            };

            Session.Save(log);

            return log.CreateModel<IFeedsLog>();
        }

        public IEnumerable<IReservationFeed> GetReservationFeeds(DateTime sd, DateTime ed, int resourceId = 0)
        {
            IQueryable<ReservationFeed> query;

            if (resourceId == 0)
                query = Session.Query<ReservationFeed>().Where(x => x.BeginDateTime >= sd && x.EndDateTime < ed);
            else
                query = Session.Query<ReservationFeed>().Where(x => x.BeginDateTime >= sd && x.EndDateTime < ed && x.ResourceID == resourceId);

            var result = query.ToList().CreateModels<IReservationFeed>();

            return result;
        }

        public IEnumerable<IReservationFeed> GetReservationFeeds(string username, DateTime sd, DateTime ed, int resourceId = 0)
        {
            IQueryable<ReservationFeed> query;

            if (resourceId == 0)
                query = Session.Query<ReservationFeed>().Where(x => (x.UserName == username || x.Invitees.Contains(username)) && x.BeginDateTime >= sd && x.EndDateTime < ed);
            else
                query = Session.Query<ReservationFeed>().Where(x => (x.UserName == username || x.Invitees.Contains(username)) && x.BeginDateTime >= sd && x.EndDateTime < ed && x.ResourceID == resourceId);

            var result = query.ToList().CreateModels<IReservationFeed>();

            return result;
        }
    }
}

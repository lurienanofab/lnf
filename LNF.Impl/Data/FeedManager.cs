using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Data;
using LNF.Scripting;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.Impl.Data
{
    public class FeedManager : ManagerBase, IFeedManager
    {
        public FeedManager(IProvider provider) : base(provider) { }

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

                DataSet ds = ExecuteQuery(feed, Parameters.Create(parameters));
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

        private DataSet ExecuteQuery(DataFeed feed, Parameters parameters)
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
                Result result = Provider.Scripting.Run(feed.FeedQuery, parameters);
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
    }
}

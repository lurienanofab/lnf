using LNF.Cache;
using LNF.Data;
using LNF.Models.Data;
using LNF.Models.Scheduler;
using LNF.Repository;
using System;
using System.Data;

namespace LNF.Scheduler.Data
{
    /// <summary>
    /// A class for handling ResourceClient data using the System.Data namespace.
    /// </summary>
    public static class ResourceClientData
    {
        /// <summary>
        /// Returns all active clients not associated with the specified resource
        /// </summary>
        public static DataTable SelectAvailClients(int resourceId)
        {
            using (var dba = DA.Current.GetAdapter())
            {
                ClientPrivilege p = ClientPrivilege.LabUser | ClientPrivilege.Staff;

                DataTable dt = dba
                    .ApplyParameters(new { Action = "SelectAvailClients", ResourceID = resourceId, Privs = (int)p })
                    .FillDataTable("sselScheduler.dbo.procResourceClientSelect");

                dt.PrimaryKey = new[] { dt.Columns["ClientID"] };

                return dt;
            }
        }

        /// <summary>
        /// Returns all active clients associated with the specified resource
        /// </summary>
        public static DataTable SelectByResource(int resourceId)
        {
            using (var dba = DA.Current.GetAdapter())
                return dba
                    .ApplyParameters(new { Action = "SelectByResource", ResourceID = resourceId })
                    .FillDataTable("sselScheduler.dbo.procResourceClientSelect");
        }

        /// <summary>
        /// Returns all resource clients and their emails and affiliations 
        /// </summary>
        public static IDataReader SelectClientList(int resourceId)
        {
            var dba = DA.Current.GetAdapter();

            return dba
                .ApplyParameters(new { Action = "SelectClientList", ResourceID = resourceId }
                ).ExecuteReader("sselScheduler.dbo.procResourceClientSelect");
        }

        /// <summary>
        /// Returns all resources associated with the specified client
        /// </summary>
        public static DataTable SelectByClient(int clientId)
        {
            using (var dba = DA.Current.GetAdapter())
                return dba
                    .ApplyParameters(new { Action = "SelectByClient", ClientID = clientId })
                    .FillDataTable("sselScheduler.dbo.procResourceClientSelect");
        }

        /// <summary>
        /// Returns all resource clients that the current client may view reserv histories of
        /// </summary>
        public static DataTable SelectReservHistoryClient(int clientId)
        {
            using (var dba = DA.Current.GetAdapter())
            {
                if (CacheManager.Current.CurrentUser.HasPriv(ClientPrivilege.Administrator | ClientPrivilege.Developer))
                    dba.SelectCommand.AddParameter("@Action", "SelectAll");
                else
                {
                    dba.SelectCommand.AddParameter("@Action", "SelectByEngineer");
                    dba.SelectCommand.AddParameter("@ClientID", clientId);
                }

                return dba.FillDataTable("sselScheduler.dbo.procResourceClientSelect");
            }
        }

        /// <summary>
        /// Returns all clients who want to receive email notification for the specified resource when a reservation is cancelled.
        /// </summary>
        public static DataTable SelectNotifyOnCancelClients(int resourceId)
        {
            using (var dba = DA.Current.GetAdapter())
                return dba
                    .ApplyParameters(new { Action = "SelectNotifyOnCancelClients", ResourceID = resourceId })
                    .FillDataTable("sselScheduler.dbo.procResourceClientSelect");
        }

        /// <summary>
        /// Returns all clients who want to receive email notification for the specified reosurce when an opening is available
        /// </summary>
        public static DataTable SelectNotifyOnOpeningClients(int resourceId)
        {
            using (var dba = DA.Current.GetAdapter())
                return dba
                    .ApplyParameters(new { Action = "SelectNotifyOnOpeningClients", ResourceID = resourceId })
                    .FillDataTable("sselScheduler.dbo.procResourceClientSelect");
        }

        /// <summary>
        /// [2009-09-16] email notify to all engineers who opted to receieive email on practice reservation
        /// </summary>
        public static DataTable SelectNotifyOnPracticeRes(int resourceId)
        {
            using (var dba = DA.Current.GetAdapter())
                return dba
                    .ApplyParameters(new { Action = "SelectNotifyOnPracticeRes", ResourceID = resourceId })
                    .FillDataTable("sselScheduler.dbo.procResourceClientSelect");
        }

        /// <summary>
        /// Returns the relationship between the specified resource and the specified client
        /// </summary>
        public static IDataReader SelectResourceClient(int resourceId, int clientId)
        {
            var dba = DA.Current.GetAdapter();

            return dba
                .ApplyParameters(new { Action = "Select", ResourceID = resourceId, ClientID = clientId })
                .ExecuteReader("sselScheduler.dbo.procResourceClientSelect");
        }

        /// <summary>
        ///  Returns all tool engineers from the specified resource
        /// </summary>
        public static DataTable SelectEngineers(int resourceId = -1)
        {
            using (var dba = DA.Current.GetAdapter())
                return dba
                    .ApplyParameters(new { Action = "SelectEngineers", ResourceID = resourceId })
                    .FillDataTable("sselScheduler.dbo.procResourceClientSelect");
        }

        /// <summary>
        /// Returns all emails from the specified resource - jiac 12/15/07
        /// </summary>
        public static DataTable SelectEmails(int resourceId, int privs)
        {
            using (var dba = DA.Current.GetAdapter())
                return dba
                    .ApplyParameters(new { ResourceID = resourceId, Privs = privs })
                    .FillDataTable("sselScheduler.dbo.procResourceEmailSelect");
        }

        /// <summary>
        /// Insert/Update/Delete resource clients
        /// </summary>
        public static void Update(DataTable dt)
        {
            using (var dba = DA.Current.GetAdapter())
            {
                dba.InsertCommand
                    .AddParameter("@Action", "InsertClient")
                    .AddParameter("@ResourceClientID", SqlDbType.Int)
                    .AddParameter("@ResourceID", SqlDbType.Int)
                    .AddParameter("@ClientID", SqlDbType.Int)
                    .AddParameter("@AuthLevel", SqlDbType.Int)
                    .AddParameter("@Expiration", SqlDbType.DateTime);

                dba.UpdateCommand
                    .AddParameter("@Action", "Update")
                    .AddParameter("@ResourceClientID", SqlDbType.Int)
                    .AddParameter("@AuthLevel", SqlDbType.Int)
                    .AddParameter("@Expiration", SqlDbType.DateTime);


                dba.DeleteCommand
                    .AddParameter("@Action", "Delete")
                    .AddParameter("@ResourceClientID", SqlDbType.Int);

                dba.UpdateDataTable(dt,
                    "sselScheduler.dbo.procResourceClientInsert",
                    "sselScheduler.dbo.procResourceClientUpdate",
                    "sselScheduler.dbo.procResourceClientDelete");
            }
        }

        public static void UpdateToolEngineers(DataTable dt)
        {
            using (var dba = DA.Current.GetAdapter())
            {
                dba.InsertCommand
                    .AddParameter("@Action", "InsertClient")
                    .AddParameter("@ResourceClientID", SqlDbType.Int)
                    .AddParameter("@ResourceID", SqlDbType.Int)
                    .AddParameter("@ClientID", SqlDbType.Int)
                    .AddParameter("@AuthLevel", ClientAuthLevel.ToolEngineer)
                    .AddParameter("@Expiration", DBNull.Value);

                dba.DeleteCommand
                    .AddParameter("@Action", "Delete")
                    .AddParameter("@ResourceClientID", SqlDbType.Int);

                dba.UpdateDataTable(dt,
                    insertSql: "sselScheduler.dbo.procResourceClientInsert",
                    deleteSql: "sselScheduler.dbo.procResourceClientDelete");
            }
        }

        /// <summary>
        /// Update Expiration date
        /// </summary>
        public static int UpdateExpiration(int resourceClientId, DateTime expirationDate)
        {
            using (var dba = DA.Current.GetAdapter())
                return dba
                    .ApplyParameters(new { Action = "UpdateExpiration", ResourceClientID = resourceClientId, Expiration = expirationDate })
                    .ExecuteNonQuery("sselScheduler.dbo.procResourceClientUpdate");
        }

        /// <summary>
        /// Updates Email Notify preference for the specified resource client
        /// </summary>
        public static int UpdateEmailNotify(int resourceClientId, int emailNotify)
        {
            using (var dba = DA.Current.GetAdapter())
                return dba
                    .ApplyParameters(new { Action = "UpdateEmailNofity", ResourceClientID = resourceClientId, EmailNotify = emailNotify })
                    .ExecuteNonQuery("sselScheduler.dbo.procResourceClientUpdate");
        }

        /// <summary>
        /// Update Practice Reservation Notification Email option
        /// </summary>
        public static int UpdatePracticeResEmailNotify(int resourceClientId, int emailNotify)
        {
            using (var dba = DA.Current.GetAdapter())
                return dba
                    .ApplyParameters(new { Action = "UpdatePracticeResEmailNotify", ResourceClientID = resourceClientId, EmailNotify = emailNotify })
                    .ExecuteNonQuery("sselScheduler.dbo.procResourceClientUpdate");
        }
    }
}

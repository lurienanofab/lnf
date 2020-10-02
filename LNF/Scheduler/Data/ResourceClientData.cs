using LNF.Data;
using LNF.Repository;
using System;
using System.Data;

namespace LNF.Scheduler.Data
{
    /// <summary>
    /// A class for handling ResourceClient data using the System.Data namespace.
    /// </summary>
    [Obsolete("What uses this? Preferences.aspx.vb")]
    public static class ResourceClientData
    {
        /// <summary>
        /// Returns all active clients not associated with the specified resource
        /// </summary>
        public static DataTable SelectAvailClients(int resourceId)
        {
            ClientPrivilege p = ClientPrivilege.LabUser | ClientPrivilege.Staff;

            var dt = DefaultDataCommand.Create()
                .Param(new { Action = "SelectAvailClients", ResourceID = resourceId, Privs = (int)p })
                .FillDataTable("sselScheduler.dbo.procResourceClientSelect");

            dt.PrimaryKey = new[] { dt.Columns["ClientID"] };

            return dt;
        }

        /// <summary>
        /// Returns all active clients associated with the specified resource
        /// </summary>
        public static DataTable SelectByResource(int resourceId)
        {
            return DefaultDataCommand.Create()
                .Param("Action", "SelectByResource")
                .Param("ResourceID", resourceId)
                .FillDataTable("sselScheduler.dbo.procResourceClientSelect");
        }

        /// <summary>
        /// Returns all resource clients and their emails and affiliations 
        /// </summary>
        public static ExecuteReaderResult SelectClientList(int resourceId)
        {
            return DefaultDataCommand.Create()
                .Param("Action", "SelectClientList")
                .Param("ResourceID", resourceId)
                .ExecuteReader("sselScheduler.dbo.procResourceClientSelect");
        }

        /// <summary>
        /// Returns all resources associated with the specified client
        /// </summary>
        public static DataTable SelectByClient(int clientId)
        {
            return DefaultDataCommand.Create()
                .Param("Action", "SelectByClient")
                .Param("ClientID", clientId)
                .FillDataTable("sselScheduler.dbo.procResourceClientSelect");
        }

        /// <summary>
        /// Returns all clients who want to receive email notification for the specified resource when a reservation is cancelled.
        /// </summary>
        public static DataTable SelectNotifyOnCancelClients(int resourceId)
        {
            return DefaultDataCommand.Create()
                .Param("Action", "SelectNotifyOnCancelClients")
                .Param("ResourceID", resourceId)
                .FillDataTable("sselScheduler.dbo.procResourceClientSelect");
        }

        /// <summary>
        /// Returns all clients who want to receive email notification for the specified reosurce when an opening is available
        /// </summary>
        public static DataTable SelectNotifyOnOpeningClients(int resourceId)
        {
            return DefaultDataCommand.Create()
                .Param("Action", "SelectNotifyOnOpeningClients")
                .Param("ResourceID", resourceId)
                .FillDataTable("sselScheduler.dbo.procResourceClientSelect");
        }

        /// <summary>
        /// [2009-09-16] email notify to all engineers who opted to receieive email on practice reservation
        /// </summary>
        public static DataTable SelectNotifyOnPracticeRes(int resourceId)
        {
            return DefaultDataCommand.Create()
                .Param("Action", "SelectNotifyOnPracticeRes")
                .Param("ResourceID", resourceId)
                .FillDataTable("sselScheduler.dbo.procResourceClientSelect");
        }

        /// <summary>
        /// Returns the relationship between the specified resource and the specified client
        /// </summary>
        public static ExecuteReaderResult SelectResourceClient(int resourceId, int clientId)
        {
            return DefaultDataCommand.Create()
                .Param(new { Action = "Select", ResourceID = resourceId, ClientID = clientId })
                .ExecuteReader("sselScheduler.dbo.procResourceClientSelect");
        }

        /// <summary>
        ///  Returns all tool engineers from the specified resource
        /// </summary>
        public static DataTable SelectEngineers(int resourceId = -1)
        {
            return DefaultDataCommand.Create()
                .Param("Action", "SelectEngineers")
                .Param("ResourceID", resourceId)
                .FillDataTable("sselScheduler.dbo.procResourceClientSelect");
        }

        /// <summary>
        /// Returns all emails from the specified resource - jiac 12/15/07
        /// </summary>
        public static DataTable SelectEmails(int resourceId, int privs)
        {
            return DefaultDataCommand.Create()
                .Param("ResourceID", resourceId)
                .Param("Privs", privs)
                .FillDataTable("sselScheduler.dbo.procResourceEmailSelect");
        }

        /// <summary>
        /// Insert/Update/Delete resource clients
        /// </summary>
        public static void Update(DataTable dt)
        {
            DefaultDataCommand.Create().Update(dt, x =>
            {
                x.Insert.SetCommandText("sselScheduler.dbo.procResourceClientInsert");
                x.Insert.AddParameter("Action", "InsertClient");
                x.Insert.AddParameter("ResourceClientID", SqlDbType.Int);
                x.Insert.AddParameter("ResourceID", SqlDbType.Int);
                x.Insert.AddParameter("ClientID", SqlDbType.Int);
                x.Insert.AddParameter("AuthLevel", SqlDbType.Int);
                x.Insert.AddParameter("Expiration", SqlDbType.DateTime);

                x.Update.SetCommandText("sselScheduler.dbo.procResourceClientUpdate");
                x.Update.AddParameter("Action", "Update");
                x.Update.AddParameter("ResourceClientID", SqlDbType.Int);
                x.Update.AddParameter("AuthLevel", SqlDbType.Int);
                x.Update.AddParameter("Expiration", SqlDbType.DateTime);

                x.Delete.SetCommandText("sselScheduler.dbo.procResourceClientDelete");
                x.Delete.AddParameter("Action", "Delete");
                x.Delete.AddParameter("ResourceClientID", SqlDbType.Int);
            });
        }

        public static void UpdateToolEngineers(DataTable dt)
        {
            DefaultDataCommand.Create().Update(dt, x =>
            {
                x.Insert.SetCommandText("sselScheduler.dbo.procResourceClientInsert");
                x.Insert.AddParameter("Action", "InsertClient");
                x.Insert.AddParameter("ResourceClientID", SqlDbType.Int);
                x.Insert.AddParameter("ResourceID", SqlDbType.Int);
                x.Insert.AddParameter("ClientID", SqlDbType.Int);
                x.Insert.AddParameter("AuthLevel", ClientAuthLevel.ToolEngineer);
                x.Insert.AddParameter("Expiration", DBNull.Value);

                x.Delete.SetCommandText("sselScheduler.dbo.procResourceClientDelete");
                x.Delete.AddParameter("Action", "Delete");
                x.Delete.AddParameter("ResourceClientID", SqlDbType.Int);
            });
        }

        /// <summary>
        /// Update Expiration date
        /// </summary>
        public static int UpdateExpiration(int resourceClientId, DateTime expirationDate)
        {
            return ServiceProvider.Current.Scheduler.Resource.UpdateExpiration(resourceClientId, expirationDate);
        }

        /// <summary>
        /// Updates Email Notify preference for the specified resource client
        /// </summary>
        public static int UpdateEmailNotify(int resourceClientId, int emailNotify)
        {
            return DefaultDataCommand.Create()
                .Param("Action", "UpdateEmailNofity")
                .Param("ResourceClientID", resourceClientId)
                .Param("EmailNotify", emailNotify)
                .ExecuteNonQuery("sselScheduler.dbo.procResourceClientUpdate").Value;
        }

        /// <summary>
        /// Update Practice Reservation Notification Email option
        /// </summary>
        public static int UpdatePracticeResEmailNotify(int resourceClientId, int emailNotify)
        {
            return DefaultDataCommand.Create()
                .Param("Action", "UpdatePracticeResEmailNotify")
                .Param("ResourceClientID", resourceClientId)
                .Param("EmailNotify", emailNotify)
                .ExecuteNonQuery("sselScheduler.dbo.procResourceClientUpdate").Value;
        }
    }
}
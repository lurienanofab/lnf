using LNF.Models.Mail;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace LNF.Impl.Mail
{
    public static class MailRepo
    {
        public static int InsertMessage(int clientId, string caller, string from, string subject, string body)
        {
            using (var conn = GetConnection())
            using (var adap = new SqlDataAdapter("INSERT Email.dbo.Message (ClientID, FromAddress, Subject, Body, Caller, CreatedOn) VALUES (@ClientID, @FromAddress, @Subject, @Body, @Caller, GETDATE()); SELECT SCOPE_IDENTITY()", conn))
            {
                conn.Open();
                adap.SelectCommand.CommandType = CommandType.Text;
                adap.SelectCommand.Parameters.AddWithValue("ClientID", clientId);
                adap.SelectCommand.Parameters.AddWithValue("FromAddress", from);
                adap.SelectCommand.Parameters.AddWithValue("Subject", subject);
                adap.SelectCommand.Parameters.AddWithValue("Body", body);
                adap.SelectCommand.Parameters.AddWithValue("Caller", caller);
                object scalar = adap.SelectCommand.ExecuteScalar();
                int result = Convert.ToInt32(scalar);
                conn.Close();
                return result;
            }
        }

        public static IEnumerable<IRecipient> SelectRecipients(int messageId)
        {
            throw new NotImplementedException();
        }

        public static int SetMessageSent(int messageId)
        {
            using (var conn = GetConnection())
            using (var adap = new SqlDataAdapter("UPDATE Email.dbo.Message SET SentOn = GETDATE() WHERE MessageID = @MessageID", conn))
            {
                conn.Open();
                adap.SelectCommand.CommandType = CommandType.Text;
                adap.SelectCommand.Parameters.AddWithValue("MessageID", messageId);
                int result = adap.SelectCommand.ExecuteNonQuery();
                conn.Close();
                return result;
            }
        }

        public static IMessage SelectMessage(int messageId)
        {
            using (var conn = GetConnection())
            using (var adap = new SqlDataAdapter("SELECT MessageID, ClientID, Caller, FromAddress, Subject, Body, Error, CreatedOn, SentOn FROM Email.dbo.Message WHERE @MessageID = MessageID", conn))
            {
                conn.Open();

                adap.SelectCommand.CommandType = CommandType.Text;
                adap.SelectCommand.Parameters.AddWithValue("MessageID", messageId);

                DataTable dt = new DataTable();
                adap.Fill(dt);

                var result = CreateMessageItems(dt);

                conn.Close();

                return result.FirstOrDefault();
            }
        }

        public static IEnumerable<IMessage> SelectMessages(DateTime sd, DateTime ed, int clientId)
        {
            using (var conn = GetConnection())
            using (var adap = new SqlDataAdapter("SELECT MessageID, ClientID, Caller, FromAddress, Subject, Body, Error, CreatedOn, SentOn FROM Email.dbo.Message WHERE CreatedOn >= @StartDate AND CreatedOn < @EndDate AND ISNULL(@ClientID, ClientID) = ClientID", conn))
            {
                adap.SelectCommand.CommandType = CommandType.Text;
                adap.SelectCommand.Parameters.AddWithValue("StartDate", sd);
                adap.SelectCommand.Parameters.AddWithValue("EndDate", ed);

                if (clientId == 0)
                    adap.SelectCommand.Parameters.AddWithValue("ClientID", DBNull.Value);
                else
                    adap.SelectCommand.Parameters.AddWithValue("ClientID", clientId);

                DataTable dt = new DataTable();
                adap.Fill(dt);

                var result = CreateMessageItems(dt);

                return result;
            }
        }

        public static int SetMessageError(int messageId, string error)
        {
            using (var conn = GetConnection())
            using (var adap = new SqlDataAdapter("UPDATE Email.dbo.Message SET Error = @Error WHERE MessageID = @MessageID", conn))
            {
                conn.Open();
                adap.SelectCommand.CommandType = CommandType.Text;
                adap.SelectCommand.Parameters.AddWithValue("MessageID", messageId);
                adap.SelectCommand.Parameters.AddWithValue("Error", error);
                int result = adap.SelectCommand.ExecuteNonQuery();
                conn.Close();
                return result;
            }
        }

        public static int InsertRecipients(int messageId, AddressType addressType, IEnumerable<string> addresses)
        {
            if (addresses == null || addresses.Count() == 0)
                return 0;

            var dt = new DataTable();

            dt.Columns.Add("MessageID", typeof(int));
            dt.Columns.Add("ClientID", typeof(int));
            dt.Columns.Add("AddressType", typeof(int));
            dt.Columns.Add("AddressText", typeof(string));

            foreach (var addr in addresses)
            {
                var ndr = dt.NewRow();
                ndr.SetField("MessageID", messageId);
                ndr.SetField("ClientID", 0);
                ndr.SetField("AddressType", (int)addressType);
                ndr.SetField("AddressText", addr);
                dt.Rows.Add(ndr);
            }

            using (var conn = GetConnection())
            using (var cmd = new SqlCommand("INSERT Email.dbo.Recipient (MessageID, ClientID, AddressType, AddressText, AddressTimestamp) VALUES (@MessageID, @ClientID, @AddressType, @AddressText, GETDATE())", conn))
            using (var adap = new SqlDataAdapter { InsertCommand = cmd })
            {
                adap.InsertCommand.CommandType = CommandType.Text;
                adap.InsertCommand.Parameters.Add("MessageID", SqlDbType.Int).SourceColumn = "MessageID";
                adap.InsertCommand.Parameters.Add("ClientID", SqlDbType.Int).SourceColumn = "ClientID";
                adap.InsertCommand.Parameters.Add("AddressType", SqlDbType.Int).SourceColumn = "AddressType";
                adap.InsertCommand.Parameters.Add("AddressText", SqlDbType.NVarChar, 100).SourceColumn = "AddressText";
                int result = adap.Update(dt);
                return result;
            }
        }

        private static IEnumerable<MessageItem> CreateMessageItems(DataTable dt)
        {
            return dt.AsEnumerable().Select(x => new MessageItem
            {
                MessageID = x.Field<int>("MessageID"),
                ClientID = x.Field<int>("ClientID"),
                Caller = x.Field<string>("Caller"),
                FromAddress = x.Field<string>("FromAddress"),
                Subject = x.Field<string>("Subject"),
                Body = x.Field<string>("Body"),
                Error = x.Field<string>("Error"),
                CreatedOn = x.Field<DateTime>("CreatedOn"),
                SentOn = x.Field<DateTime?>("SentOn")
            });
        }

        private static SqlConnection GetConnection()
        {
            var result = new SqlConnection(ConfigurationManager.ConnectionStrings["cnSselData"].ConnectionString);
            return result;
        }
    }
}
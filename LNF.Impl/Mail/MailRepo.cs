using LNF.Mail;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace LNF.Impl.Mail
{
    public class MailRepo
    {
        private readonly SqlConnection _conn;
        private readonly SqlTransaction _tx;

        private static SqlConnection NewConnection()
        {
            var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["cnSselData"].ConnectionString);
            conn.Open();
            return conn;
        }

        public static MailRepo Create() => Create(NewConnection());

        public static MailRepo Create(SqlConnection conn, SqlTransaction tx = null)
        {
            return new MailRepo(conn, tx);
        }

        private MailRepo(SqlConnection conn, SqlTransaction tx)
        {
            _conn = conn;
            _tx = tx;
        }

        public int InsertMessage(int clientId, string caller, string from, string subject, string body)
        {
            using (var cmd = GetCommand("INSERT Email.dbo.Message (ClientID, FromAddress, Subject, Body, Caller, CreatedOn) VALUES (@ClientID, @FromAddress, @Subject, @Body, @Caller, GETDATE()); SELECT SCOPE_IDENTITY()", CommandType.Text))
            {

                cmd.Parameters.AddWithValue("ClientID", clientId);
                cmd.Parameters.AddWithValue("FromAddress", from);
                cmd.Parameters.AddWithValue("Subject", subject);
                cmd.Parameters.AddWithValue("Body", body);
                cmd.Parameters.AddWithValue("Caller", caller);
                object scalar = cmd.ExecuteScalar();
                int result = Convert.ToInt32(scalar);
                return result;
            }
        }

        public IEnumerable<Recipient> SelectRecipients(int messageId)
        {
            using (var cmd = GetCommand("SELECT * FROM Email.dbo.Recipient WHERE MessageID = @MessageID", CommandType.Text))
            using (var adap = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("MessageID", messageId);
                var dt = new DataTable();
                adap.Fill(dt);

                var result = new List<Recipient>();

                foreach (DataRow dr in dt.Rows)
                {
                    result.Add(new Recipient
                    {
                        RecipientID = dr.Field<int>("RecipientID"),
                        MessageID = dr.Field<int>("MessageID"),
                        ClientID = dr.Field<int>("ClientID"),
                        AddressType = dr.Field<AddressType>("AddressType"),
                        AddressText = dr.Field<string>("AddressText"),
                        AddressTimestamp = dr.Field<DateTime>("AddressTimestamp")
                    });
                }

                return result;
            }
        }

        public int SetMessageSent(int messageId)
        {
            using (var cmd = GetCommand("UPDATE Email.dbo.Message SET SentOn = GETDATE() WHERE MessageID = @MessageID", CommandType.Text))
            {
                cmd.Parameters.AddWithValue("MessageID", messageId);
                int result = cmd.ExecuteNonQuery();
                return result;
            }
        }

        public Message SelectMessage(int messageId)
        {
            using (var cmd = GetCommand("SELECT MessageID, ClientID, Caller, FromAddress, Subject, Body, Error, CreatedOn, SentOn FROM Email.dbo.Message WHERE @MessageID = MessageID", CommandType.Text))
            using (var adap = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("MessageID", messageId);

                DataTable dt = new DataTable();
                adap.Fill(dt);

                var result = CreateMessageItems(dt);

                return result.FirstOrDefault();
            }
        }

        public IEnumerable<Message> SelectMessages(DateTime sd, DateTime ed, int clientId)
        {
            using (var cmd = GetCommand("SELECT MessageID, ClientID, Caller, FromAddress, Subject, Body, Error, CreatedOn, SentOn FROM Email.dbo.Message WHERE CreatedOn >= @StartDate AND CreatedOn < @EndDate AND ISNULL(@ClientID, ClientID) = ClientID", CommandType.Text))
            using (var adap = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("StartDate", sd);
                cmd.Parameters.AddWithValue("EndDate", ed);

                if (clientId == 0)
                    cmd.Parameters.AddWithValue("ClientID", DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("ClientID", clientId);

                DataTable dt = new DataTable();
                adap.Fill(dt);

                var result = CreateMessageItems(dt);

                return result;
            }
        }

        public int SetMessageError(int messageId, string error)
        {
            using (var cmd = GetCommand("UPDATE Email.dbo.Message SET Error = @Error WHERE MessageID = @MessageID", CommandType.Text))
            {
                cmd.Parameters.AddWithValue("MessageID", messageId);
                cmd.Parameters.AddWithValue("Error", error);
                int result = cmd.ExecuteNonQuery();
                return result;
            }
        }

        public int InsertRecipients(int messageId, AddressType addressType, IEnumerable<string> addresses)
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

            using (var cmd = GetCommand("INSERT Email.dbo.Recipient (MessageID, ClientID, AddressType, AddressText, AddressTimestamp) VALUES (@MessageID, @ClientID, @AddressType, @AddressText, GETDATE())", CommandType.Text))
            using (var adap = new SqlDataAdapter { InsertCommand = cmd })
            {
                cmd.Parameters.Add("MessageID", SqlDbType.Int).SourceColumn = "MessageID";
                cmd.Parameters.Add("ClientID", SqlDbType.Int).SourceColumn = "ClientID";
                cmd.Parameters.Add("AddressType", SqlDbType.Int).SourceColumn = "AddressType";
                cmd.Parameters.Add("AddressText", SqlDbType.NVarChar, 100).SourceColumn = "AddressText";
                int result = adap.Update(dt);
                return result;
            }
        }

        private IEnumerable<Message> CreateMessageItems(DataTable dt)
        {
            return dt.AsEnumerable().Select(x => new Message
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
            }).ToList();
        }

        private SqlConnection GetConnection() => _conn;

        private SqlCommand GetCommand(string sql, CommandType commandType = CommandType.StoredProcedure)
        {
            var conn = GetConnection();

            if (conn.State != ConnectionState.Open)
                conn.Open();

            var cmd = new SqlCommand(sql, conn, _tx) { CommandType = commandType };

            return cmd;
        }

    }
}
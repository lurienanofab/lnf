using LNF.Data;
using LNF.Repository;
using System;
using System.Data;

namespace LNF.CommonTools
{
    public class AuthUtility
    {
        private readonly IClientRepository repo;
        private DataTable dt;
        int z = 1;
        public AuthUtility(IClientRepository repo)
        {
            this.repo = repo;
        }

        private DataTable GetData(int clientId)
        {
            if (dt == null)
            { 
                dt = DataCommand.Create(CommandType.Text)
                    .Param("ClientID", clientId)
                    .FillDataTable("SELECT ClientID, Password, PasswordHash FROM sselData.dbo.Client WHERE ClientID = @ClientID");
            }

            return dt;
        }

        public IClient Login(string username, string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new Exception("Invalid password.");

            var client = repo.GetClient(username);

            if (client == null)
                throw new Exception("Invalid username.");

            if (LoginPasswordCheck(client.ClientID, password))
                return client;
            else
                throw new Exception("Invalid password.");
        }

        public static bool IsUniveralPassword(string password)
        {
            return password == Configuration.Current.DataAccess.UniversalPassword;
        }

        public bool LoginPasswordCheck(int clientId, string password)
        {
            if (IsUniveralPassword(password))
                return true;

            if (PasswordResetRequired(clientId))
                return false;

            return PasswordCheck(clientId, password);
        }

        public bool PasswordCheck(int clientId, string password)
        {
            DataTable dt = GetData(clientId);

            if (dt.Rows.Count == 0)
                throw new ItemNotFoundException("Client", "ClientID", clientId);

            var pw = Convert.ToString(dt.Rows[0]["Password"]);
            var salt = Convert.ToString(dt.Rows[0]["PasswordHash"]);

            // handle new password
            var encrypted = Encryption.SHA256.EncryptText(password + salt);
            var result = pw == encrypted;
            return result;
        }

        ///// <summary>
        ///// Sets the user's password.
        ///// </summary>
        public int SetPassword(int clientId, string password)
        {
            var salt = Guid.NewGuid().ToString("n");
            var pw = Encryption.SHA256.EncryptText(password + salt);

            var sql = "UPDATE sselData.dbo.Client SET Password = @Password, PasswordHash = @PasswordHash WHERE ClientID = @ClientID";

            int result = DataCommand.Create(CommandType.Text)
                .Param("ClientID", clientId)
                .Param("Password", pw)
                .Param("PasswordHash", salt)
                .ExecuteNonQuery(sql).Value;

            return result;
        }

        public bool PasswordResetRequired(int clientId)
        {
            DataTable dt = GetData(clientId);

            var pwd = Convert.ToString(dt.Rows[0]["Password"]);
            var salt = Convert.ToString(dt.Rows[0]["PasswordHash"]);

            if (string.IsNullOrEmpty(pwd) || string.IsNullOrEmpty(salt))
                return true;

            if (pwd.Length < 64)
                return true;

            return false;
        }

        public void ResetPassword(int clientId)
        {
            var sql = "UPDATE sselData.dbo.Client SET Password = @Password, PasswordHash = @PasswordHash WHERE ClientID = @ClientID";

            DataCommand.Create(CommandType.Text)
                .Param("ClientID", clientId)
                .Param("Password", DBNull.Value)
                .Param("PasswordHash", DBNull.Value)
                .ExecuteNonQuery(sql);
        }
    }
}
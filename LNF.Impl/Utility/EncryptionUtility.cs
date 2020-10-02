using LNF.Util.Encryption;
using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace LNF.Impl.Util
{
    public class EncryptionUtility : IEncryptionUtility
    {
        private static string SecretKey
        {
            get
            {
                throw new NotImplementedException();
                //string result = ConfigurationManager.AppSettings["SecretKey"];

                //if (string.IsNullOrEmpty(result))
                //    throw new Exception("Missing required appSetting: SecretKey");

                //return result;
            }
        }

        //public string EncryptText(string text)
        //{
        //    byte[] byKey = { };
        //    byKey = Encoding.UTF8.GetBytes(SecretKey.Substring(0, 8));

        //    byte[] IV = { 0x56, 0x18, 0x9C, 0xFD, 0x38, 0xB5, 0xAA, 0x61 };
        //    DESCryptoServiceProvider des = new DESCryptoServiceProvider();
        //    MemoryStream ms = new MemoryStream();
        //    CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(byKey, IV), CryptoStreamMode.Write);

        //    byte[] inputByteArray = Encoding.UTF8.GetBytes(text);
        //    cs.Write(inputByteArray, 0, inputByteArray.Length);
        //    cs.FlushFinalBlock();

        //    return Convert.ToBase64String(ms.ToArray());
        //}

        public string EncryptText(string text)
        {
            // Create a SHA256   
            using (SHA256 sha256 = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public string DecryptText(string text)
        {
            throw new NotImplementedException();
            //byte[] byKey = { };
            //byte[] inputByteArray = new byte[text.Length];
            //inputByteArray = Convert.FromBase64String(text);

            //byKey = Encoding.UTF8.GetBytes(SecretKey.Substring(0, 8));

            //byte[] IV = { 0x56, 0x18, 0x9C, 0xFD, 0x38, 0xB5, 0xAA, 0x61 };
            //DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            //MemoryStream ms = new MemoryStream();
            //CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(byKey, IV), CryptoStreamMode.Write);

            //cs.Write(inputByteArray, 0, inputByteArray.Length);
            //cs.FlushFinalBlock();

            //return Encoding.UTF8.GetString(ms.ToArray());
        }

        public string Hash(string input)
        {
            MD5 hasher = MD5.Create();
            byte[] data = hasher.ComputeHash(Encoding.UTF8.GetBytes(SecretKey + input));

            string result = string.Empty;

            foreach (byte b in data)
            {
                if (b < 16)
                    result += "0" + b.ToString("x");
                else
                    result += b.ToString("x");
            }

            return result;
        }
    }
}

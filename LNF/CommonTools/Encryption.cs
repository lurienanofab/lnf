using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace LNF.CommonTools
{

    public static class Encryption
    {
        public static SHA256Encryption SHA256 { get; } = new SHA256Encryption();
        [Obsolete] public static DESEncryption DES { get; } = new DESEncryption();
        [Obsolete] public static MD5Encryption MD5 { get; } = new MD5Encryption();
    }

    public abstract class EncryptionBase
    {
        [Obsolete]
        protected string SecretKey
        {
            get
            {
                string result = ConfigurationManager.AppSettings["SecretKey"];

                if (string.IsNullOrEmpty(result))
                    throw new InvalidOperationException("Missing appSetting: SecretKey");

                return result;
            }
        }
    }

    public class SHA256Encryption
    {
        internal SHA256Encryption() { }

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
    }

    [Obsolete]
    public class DESEncryption : EncryptionBase
    {
        internal DESEncryption() { }

        // The function used to encrypt the text
        public string EncryptText(string text)
        {
            // This is the old encryption method keeping until all old passwords are changed.

            byte[] byKey = Encoding.UTF8.GetBytes(SecretKey.Substring(0, 8));

            byte[] IV = { 0x56, 0x18, 0x9C, 0xFD, 0x38, 0xB5, 0xAA, 0x61 };
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(byKey, IV), CryptoStreamMode.Write);

            byte[] inputByteArray = Encoding.UTF8.GetBytes(text);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();

            return Convert.ToBase64String(ms.ToArray());
        }

        //The function used to decrypt the text
        public string DecryptText(string text)
        {
            byte[] byKey = Encoding.UTF8.GetBytes(SecretKey.Substring(0, 8));
            byte[] inputByteArray = Convert.FromBase64String(text);

            byte[] IV = { 0x56, 0x18, 0x9C, 0xFD, 0x38, 0xB5, 0xAA, 0x61 };
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(byKey, IV), CryptoStreamMode.Write);

            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();

            return Encoding.UTF8.GetString(ms.ToArray());
        }
    }

    [Obsolete]
    public class MD5Encryption : EncryptionBase
    {
        internal MD5Encryption() { }

        public string Hash(string input)
        {
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(SecretKey + input));

            string result = string.Empty;

            foreach (byte b in hash)
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

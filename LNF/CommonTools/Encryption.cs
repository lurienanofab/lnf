using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Configuration;

namespace LNF.CommonTools
{
    public class Encryption
    {
        private static string SecretKey
        {
            get
            {
                string result = ConfigurationManager.AppSettings["SecretKey"];

                if (string.IsNullOrEmpty(result))
                    throw new InvalidOperationException("Missing appSetting: SecretKey");

                return result;
            }
        }

        // Encrypt the text
        public string EncryptText(string strText)
        {
            return Encrypt(strText, SecretKey);
        }

        // Decrypt the text 
        public string DecryptText(string strText)
        {
            return Decrypt(strText, SecretKey);
        }

        // The function used to encrypt the text
        private string Encrypt(string strText, string strEncrKey)
        {
            byte[] byKey = Encoding.UTF8.GetBytes(strEncrKey.Substring(0, 8));

            byte[] IV = { 0x56, 0x18, 0x9C, 0xFD, 0x38, 0xB5, 0xAA, 0x61 };
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(byKey, IV), CryptoStreamMode.Write);

            byte[] inputByteArray = Encoding.UTF8.GetBytes(strText);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();

            return Convert.ToBase64String(ms.ToArray());
        }

        //The function used to decrypt the text
        private string Decrypt(string strText, string sDecrKey)
        {
            byte[] byKey = Encoding.UTF8.GetBytes(sDecrKey.Substring(0, 8));
            byte[] inputByteArray = Convert.FromBase64String(strText);

            byte[] IV = { 0x56, 0x18, 0x9C, 0xFD, 0x38, 0xB5, 0xAA, 0x61 };
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(byKey, IV), CryptoStreamMode.Write);

            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();

            return Encoding.UTF8.GetString(ms.ToArray());
        }

        public static string MD5(string input)
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

        public static string SHA256(string input)
        {
            SHA256Managed sha256 = new SHA256Managed();
            byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(SecretKey + input));

            string result = string.Empty;

            foreach (byte x in hash)
            {
                result += string.Format("{0:x2}", x);
            }

            return result;
        }
    }
}

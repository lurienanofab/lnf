using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace LNF.CommonTools
{
    public class Encryption
    {
        // Encrypt the text
        public string EncryptText(string strText)
        {
            return Encrypt(strText, "NiNnPaSs");
        }

        // Decrypt the text 
        public string DecryptText(string strText)
        {
            return Decrypt(strText, "NiNnPaSs");
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

        public static string Hash(string input)
        {
            MD5 hasher = MD5.Create();
            string salt = "NiNnPaSs";
            byte[] data = hasher.ComputeHash(Encoding.UTF8.GetBytes(salt + input));
            string result = Encryption.BytesToHexString(data);
            return result;
        }

        public static string BytesToHexString(byte[] data)
        {
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;

namespace LNF.Ordering
{
    public class Attachment
    {
        public int POID { get; set; }

        public string FilePath { get; set; }

        public string FileName { get { return Path.GetFileName(FilePath); } }

        public string Url { get { return GetUrl(POID, FileName); } }

        public static IEnumerable<int> GetFolders()
        {
            string root = ConfigurationManager.AppSettings["AttachmentsDirectory"];

            if (root.StartsWith("."))
                root = Providers.Context.Current.GetServerPath(root);

            List<int> result = new List<int>();

            if (Directory.Exists(root))
            {
                string[] dirs = Directory.GetDirectories(root);
                foreach (string dirPath in dirs)
                {
                    string dirName = Path.GetFileName(dirPath);

                    int poid;

                    if (int.TryParse(dirName, out poid))
                        if (Directory.GetFiles(dirPath).Length > 0)
                            result.Add(poid);
                }
            }

            return result.ToArray();
        }

        public static IEnumerable<Attachment> GetAttachments(int poid)
        {
            if (poid == 0)
                throw new ArgumentException("The argument poid cannot be zero.", "poid");

            string path = GetPhysicalPath(poid);

            if (Directory.Exists(path))
            {
                string[] files = Directory.GetFiles(path);

                if (files.Length > 0)
                    return files.Select(x => new Attachment() { FilePath = x, POID = poid }).ToArray();
            }

            return new Attachment[] { };
        }

        public static string GetPhysicalPath(int poid)
        {
            string result = Path.Combine(ConfigurationManager.AppSettings["AttachmentsDirectory"], poid.ToString());

            if (result.StartsWith("."))
                result = Providers.Context.Current.GetServerPath(result);

            if (!Directory.Exists(result))
                Directory.CreateDirectory(result);

            return result;
        }

        public static string GetPhysicalPath(int poid, string fileName)
        {
            string result = Path.Combine(GetPhysicalPath(poid), fileName);
            return result;
        }

        public static string GetUrl(int poid, string fileName)
        {
            string vp = ConfigurationManager.AppSettings["AttachmentsVirtualPath"]; //should contain {0} and {1} to be replaced by POID and FileName
            string absolutePath = Providers.Context.Current.GetAbsolutePath(string.Format(vp, poid, fileName));
            return Providers.Context.Current.GetRequestUrl().GetLeftPart(UriPartial.Authority) + absolutePath;
        }

        public static byte[] GetBytes(int poid, string fileName)
        {
            return File.ReadAllBytes(GetPhysicalPath(poid, fileName));
        }

        public static bool Exists(int poid, string fileName)
        {
            return File.Exists(GetPhysicalPath(poid, fileName));
        }

        public static void DeleteAttachment(int poid, string fileName)
        {
            string filePath = GetPhysicalPath(poid, fileName);
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }
}

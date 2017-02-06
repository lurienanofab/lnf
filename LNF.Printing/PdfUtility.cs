using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace LNF.Printing
{
    public static class PdfUtility
    {
        public static byte[] Combine(IEnumerable<byte[]> files)
        {
            if (files.Count() > 1)
            {
                MemoryStream ms = new MemoryStream();
                Document doc = new Document();
                PdfCopy copy = new PdfCopy(doc, ms);
                doc.Open();
                foreach (byte[] f in files)
                    copy.AddDocument(new PdfReader(f));
                doc.Close();
                return ms.ToArray();
            }
            else if (files.Count() == 1)
                return files.ElementAt(0);
            else
                return null;
        }

        public static byte[] zCombinePdfs(List<byte[]> files)
        {
            if (files.Count > 1)
            {
                MemoryStream msOutput = new MemoryStream();

                PdfReader reader = new PdfReader(files[0]);

                Document doc = new Document();

                PdfSmartCopy copy = new PdfSmartCopy(doc, msOutput);

                doc.Open();

                for (int k = 0; k < files.Count; k++)
                {
                    for (int i = 1; i < reader.NumberOfPages + 1; i++)
                    {
                        copy.AddPage(copy.GetImportedPage(reader, i));
                        copy.FreeReader(reader);
                    }
                }

                reader.Close();
                copy.Close();
                doc.Close();

                return msOutput.ToArray();
            }
            else if (files.Count == 1)
            {
                return files[0];
            }

            return null;
        }
    }
}

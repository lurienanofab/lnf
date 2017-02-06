using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Drive.v2.Data;

namespace LNF.GoogleApi
{
    public class DriveFile
    {
        private File _file;

        internal DriveFile(File file)
        {
            _file = file;
        }

        public string Title
        {
            get { return _file.Title; }
            set { _file.Title = value; }
        }

        public string MimeType
        {
            get { return _file.MimeType; }
        }

        public DateTime? Created
        {
            get { return _file.CreatedDate; }
        }

        public DateTime? Modified
        {
            get { return _file.ModifiedDate; }
        }

        public static DriveFile CreateNew(string mimeType)
        {
            var file = new File();
            file.MimeType = mimeType;
            return new DriveFile(file);
        }

        internal File GetFile()
        {
            return _file;
        }
    }
}

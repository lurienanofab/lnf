using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Google.Apis.Drive.v2;

namespace LNF.GoogleApi
{
    public class DriveClient
    {
        private GoogleAuthorization _auth;

        public Controller Controller { get; private set; }

        public DriveClient(GoogleAuthorization auth, Controller controller)
        {
            _auth = auth;
            Controller = controller;
        }

        private DriveService _service;

        private DriveService GetDriveService()
        {
            if (_service == null)
                _service = new DriveService(_auth.GetInitializer("LNF Online Services", Controller));

            return _service;
        }

        public async Task<IEnumerable<DriveFile>> ListFiles()
        {
            var list = await GetDriveService().Files.List().ExecuteAsync();
            return list.Items.Select(x => new DriveFile(x));
        }

        public DriveFile CreateSpreadsheet(string title, System.Web.Mvc.Controller controller)
        {
            var result = DriveFile.CreateNew("application/vnd.google-apps.spreadsheet");
            result.Title = title;
            GetDriveService().Files.Insert(result.GetFile());
            return result;
        }
    }
}

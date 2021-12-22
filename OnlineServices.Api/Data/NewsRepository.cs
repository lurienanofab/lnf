using LNF.Data;
using RestSharp;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Data
{
    public class NewsRepository : ApiClient, INewsRepository
    {
        public NewsRepository(IRestClient rc) : base(rc) { }

        public void Delete(int newsId, int currentUserClientId)
        {
            throw new NotImplementedException();
        }

        public int DeleteImage(int newsId, int currentUserClientId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<INews> FindByStatus(string status)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<NewsListItem> GetActive()
        {
            throw new NotImplementedException();
        }

        public void SetDefault(int newsId, int currentUserClientId)
        {
            throw new NotImplementedException();
        }

        public void Undelete(int newsId, int currentUserClientId)
        {
            throw new NotImplementedException();
        }

        public int UploadImage(int newsId, byte[] image, string fileName, string contentType, int currentUserClientId)
        {
            throw new NotImplementedException();
        }
    }
}

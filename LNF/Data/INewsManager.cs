using LNF.Repository;
using LNF.Repository.Data;
using System.Collections.Generic;

namespace LNF.Data
{
    public interface INewsManager : IManager
    {
        void Delete(News item, int currentUserClientId);
        int DeleteImage(int newsId, int currentUserClientId);
        IList<News> FindByStatus(string status);
        void SetDefault(News item, int currentUserClientId);
        void Undelete(News item, int currentUserClientId);
        int UploadImage(int newsId, byte[] image, string fileName, string contentType, int currentUserClientId);
    }
}
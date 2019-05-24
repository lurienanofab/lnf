using LNF.Models.Data;
using System.Collections.Generic;

namespace LNF.Data
{
    public interface INewsManager
    {
        void Delete(int newsId, int currentUserClientId);
        int DeleteImage(int newsId, int currentUserClientId);
        IEnumerable<INews> FindByStatus(string status);
        void SetDefault(int newsId, int currentUserClientId);
        void Undelete(int newsId, int currentUserClientId);
        int UploadImage(int newsId, byte[] image, string fileName, string contentType, int currentUserClientId);
    }
}
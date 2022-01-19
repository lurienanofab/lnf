using System.Collections.Generic;

namespace LNF.Data
{
    public interface INewsRepository
    {
        News GetNewsItem(int newsId);
        void Delete(int newsId, int currentUserClientId);
        int DeleteImage(int newsId, int currentUserClientId);
        IEnumerable<News> FindByStatus(string status);
        void SetDefault(int newsId, int currentUserClientId);
        void Undelete(int newsId, int currentUserClientId);
        int UploadImage(int newsId, byte[] image, string fileName, string contentType, int currentUserClientId);
        IEnumerable<NewsListItem> GetActive();
    }
}
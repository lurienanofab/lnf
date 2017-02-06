using LNF.Cache;
using LNF.Repository;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Data
{
    public static class NewsUtility
    {
        public static IList<News> FindByStatus(string status)
        {
            IList<News> activeItems = DA.Current.Query<News>().Where(x =>
                (x.NewsPublishDate == null || x.NewsPublishDate.Value <= DateTime.Now) &&
                (x.NewsExpirationDate == null || x.NewsExpirationDate > DateTime.Now) &&
                x.NewsActive && !x.NewsDeleted).ToList();

            if (activeItems.Where(x => !x.NewsTicker).Count() == 0)
                activeItems.Add(DA.Current.Query<News>().FirstOrDefault(x => x.NewsDefault));

            switch (status)
            {
                case "current":
                    return activeItems;
                case "inactive":
                    IList<News> inactiveItems = DA.Current.Query<News>().Where(x => !x.NewsDeleted).ToList();
                    return inactiveItems
                        .Where(x => !activeItems.Select(a => a.NewsID).Contains(x.NewsID))
                        .ToList();
                default:
                    throw new ArgumentException("Argument status must be either \"current\" or \"inactive\".");
            }
        }

        public static int UploadImage(int newsId, byte[] image, string fileName, string contentType)
        {
            News n = DA.Current.Single<News>(newsId);
            if (n != null)
            {
                n.NewsUpdatedByClientID = CacheManager.Current.CurrentUser.ClientID;
                n.NewsLastUpdate = DateTime.Now;
                n.NewsImage = image;
                n.NewsImageFileName = fileName;
                n.NewsImageContentType = contentType;
                return 1;
            }
            return 0;
        }

        public static int DeleteImage(int newsId)
        {
            News n = DA.Current.Single<News>(newsId);
            if (n != null)
            {
                n.NewsUpdatedByClientID = CacheManager.Current.CurrentUser.ClientID;
                n.NewsLastUpdate = DateTime.Now;
                n.NewsImage = null;
                n.NewsImageFileName = null;
                n.NewsImageContentType = null;
                return 1;
            }
            return 0;
        }
    }
}

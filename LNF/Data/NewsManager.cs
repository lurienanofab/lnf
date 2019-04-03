using LNF.Repository;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Data
{
    public class NewsManager : ManagerBase, INewsManager
    {
        public NewsManager(IProvider provider) : base(provider) { }

        /// <summary>
        /// Deletes a News item
        /// </summary>
        public void Delete(News item, int currentUserClientId)
        {
            item.NewsUpdatedByClientID = currentUserClientId;
            item.NewsLastUpdate = DateTime.Now;
            item.NewsDeleted = true;
        }

        /// <summary>
        /// Restores a previously deleted News item
        /// </summary>
        public void Undelete(News item, int currentUserClientId)
        {
            item.NewsUpdatedByClientID = currentUserClientId;
            item.NewsLastUpdate = DateTime.Now;
            item.NewsDeleted = false;
        }

        public void SetDefault(News item, int currentUserClientId)
        {
            News d = Session.Query<News>().FirstOrDefault(x => x.NewsDefault);
            d.NewsDefault = false;

            item.NewsUpdatedByClientID = currentUserClientId;
            item.NewsLastUpdate = DateTime.Now;
            item.NewsDefault = true;
        }

        public IList<News> FindByStatus(string status)
        {
            IList<News> activeItems = Session.Query<News>().Where(x =>
                (x.NewsPublishDate == null || x.NewsPublishDate.Value <= DateTime.Now) &&
                (x.NewsExpirationDate == null || x.NewsExpirationDate > DateTime.Now) &&
                x.NewsActive && !x.NewsDeleted).ToList();

            if (activeItems.Where(x => !x.NewsTicker).Count() == 0)
                activeItems.Add(Session.Query<News>().FirstOrDefault(x => x.NewsDefault));

            switch (status)
            {
                case "current":
                    return activeItems;
                case "inactive":
                    IList<News> inactiveItems = Session.Query<News>().Where(x => !x.NewsDeleted).ToList();
                    return inactiveItems
                        .Where(x => !activeItems.Select(a => a.NewsID).Contains(x.NewsID))
                        .ToList();
                default:
                    throw new ArgumentException("Argument status must be either \"current\" or \"inactive\".");
            }
        }

        public int UploadImage(int newsId, byte[] image, string fileName, string contentType, int currentUserClientId)
        {
            News n = Session.Single<News>(newsId);

            if (n != null)
            {
                n.NewsUpdatedByClientID = currentUserClientId;
                n.NewsLastUpdate = DateTime.Now;
                n.NewsImage = image;
                n.NewsImageFileName = fileName;
                n.NewsImageContentType = contentType;
                return 1;
            }

            return 0;
        }

        public int DeleteImage(int newsId, int currentUserClientId)
        {
            News n = Session.Single<News>(newsId);
            if (n != null)
            {
                n.NewsUpdatedByClientID = currentUserClientId;
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

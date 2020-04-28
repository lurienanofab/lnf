using LNF.Data;
using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using LNF.Impl.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Impl.Data
{
    public class NewsRepository : RepositoryBase, INewsRepository
    {
        public NewsRepository(ISessionManager mgr) : base(mgr) { }

        /// <summary>
        /// Deletes a News item
        /// </summary>
        public void Delete(int newsId, int currentUserClientId)
        {
            var n = GetNews(newsId);

            n.NewsUpdatedByClientID = currentUserClientId;
            n.NewsLastUpdate = DateTime.Now;
            n.NewsDeleted = true;

            Session.SaveOrUpdate(n);
        }

        /// <summary>
        /// Restores a previously deleted News item
        /// </summary>
        public void Undelete(int newsId, int currentUserClientId)
        {
            var n = GetNews(newsId);

            n.NewsUpdatedByClientID = currentUserClientId;
            n.NewsLastUpdate = DateTime.Now;
            n.NewsDeleted = false;

            Session.SaveOrUpdate(n);
        }

        public void SetDefault(int newsId, int currentUserClientId)
        {
            var d = Session.Query<News>().FirstOrDefault(x => x.NewsDefault);

            if (d != null)
            {
                d.NewsDefault = false;
                Session.SaveOrUpdate(d);
            }

            var n = GetNews(newsId);

            n.NewsUpdatedByClientID = currentUserClientId;
            n.NewsLastUpdate = DateTime.Now;
            n.NewsDefault = true;

            Session.SaveOrUpdate(n);
        }

        public IEnumerable<INews> FindByStatus(string status)
        {
            var activeItems = Session.Query<News>().Where(x =>
                (x.NewsPublishDate == null || x.NewsPublishDate.Value <= DateTime.Now) &&
                (x.NewsExpirationDate == null || x.NewsExpirationDate > DateTime.Now) &&
                x.NewsActive && !x.NewsDeleted).ToList();

            if (activeItems.Where(x => !x.NewsTicker).Count() == 0)
                activeItems.Add(Session.Query<News>().FirstOrDefault(x => x.NewsDefault));

            switch (status)
            {
                case "current":
                    return activeItems.CreateModels<INews>();
                case "inactive":
                    var inactiveItems = Session.Query<News>().Where(x => !x.NewsDeleted).ToList();
                    return inactiveItems
                        .Where(x => !activeItems.Select(a => a.NewsID).Contains(x.NewsID))
                        .CreateModels<INews>();
                default:
                    throw new ArgumentException("Argument status must be either \"current\" or \"inactive\".");
            }
        }

        public int UploadImage(int newsId, byte[] image, string fileName, string contentType, int currentUserClientId)
        {
            var n = Session.Get<News>(newsId);

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
            var n = Session.Get<News>(newsId);
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

        private News GetNews(int newsId)
        {
            var result = Session.Get<News>(newsId);

            if (result == null)
                throw new ItemNotFoundException<News>(x => x.NewsID, newsId);

            return result;
        }
    }
}

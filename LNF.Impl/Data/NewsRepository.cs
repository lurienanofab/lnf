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

        public News GetNewsItem(int itemId)
        {
            var result = Session.Get<News>(itemId);
            return result;
        }

        public IEnumerable<NewsListItem> GetActive()
        {
            var now = DateTime.Now;
            var query = Session.Query<News>().Where(x => x.NewsPublishDate <= now && (x.NewsExpirationDate == null || x.NewsExpirationDate > now) && x.NewsActive && !x.NewsDeleted).ToList();
            var result = query.Select(CreateNewsListItem).ToList();
            return result;
        }

        /// <summary>
        /// Deletes a News item
        /// </summary>
        public void Delete(int newsId, int currentUserClientId)
        {
            var n = Require<News>(newsId);

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
            var n = Require<News>(newsId);

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

            var n = Require<News>(newsId);

            n.NewsUpdatedByClientID = currentUserClientId;
            n.NewsLastUpdate = DateTime.Now;
            n.NewsDefault = true;

            Session.SaveOrUpdate(n);
        }

        public IEnumerable<News> FindByStatus(string status)
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
                    return activeItems;
                case "inactive":
                    var inactiveItems = Session.Query<News>().Where(x => !x.NewsDeleted).ToList();
                    return inactiveItems
                        .Where(x => !activeItems.Select(a => a.NewsID).Contains(x.NewsID))
                        .ToList();
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

        private NewsListItem CreateNewsListItem(News x)
        {
            return new NewsListItem
            {
                NewsID = x.NewsID,
                Text = x.NewsDescription,
                ImageUrl = GetImageUrl(x),
                IsTicker = x.NewsTicker
            };
        }

        private string GetImageUrl(News x)
        {
            if (!x.NewsTicker && x.NewsImage != null && x.NewsImage.Count() > 0)
                return $"news/image/{x.NewsID}";
            else
                return string.Empty;
        }

        protected News CreateNewsItem(News x)
        {
            return new News
            {
                NewsID = x.NewsID,
                NewsCreatedByClientID = x.NewsCreatedByClientID,
                NewsUpdatedByClientID = x.NewsUpdatedByClientID,
                NewsImage = x.NewsImage,
                NewsImageFileName = x.NewsImageFileName,
                NewsImageContentType = x.NewsImageContentType,
                NewsTitle = x.NewsTitle,
                NewsDescription = x.NewsDescription,
                NewsCreatedDate = x.NewsCreatedDate,
                NewsLastUpdate = x.NewsLastUpdate,
                NewsPublishDate = x.NewsPublishDate,
                NewsExpirationDate = x.NewsExpirationDate,
                NewsSortOrder = x.NewsSortOrder,
                NewsTicker = x.NewsTicker,
                NewsDefault = x.NewsDefault,
                NewsActive = x.NewsActive,
                NewsDeleted = x.NewsDeleted
            };
        }
    }
}

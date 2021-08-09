using System;

namespace LNF.Impl.Mongo
{
    public class UpdateResult
    {
        private readonly MongoDB.Driver.UpdateResult _res;

        public bool IsAcknowledged => _res.IsAcknowledged;
        public long MatchedCount => _res.MatchedCount;
        public long ModifiedCount => _res.ModifiedCount;

        internal UpdateResult(MongoDB.Driver.UpdateResult res)
        {
            _res = res ?? throw new ArgumentException("res");
        }
    }
}

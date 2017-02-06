using LNF.Models.Scheduler;
using System;
using System.Collections.Generic;

namespace LNF.Repository.Scheduler
{
    public class AuthLevel : IDataItem
    {
        public virtual ClientAuthLevel AuthLevelID { get; set; }
        public virtual int Authorizable { get; set; }
        public virtual string AuthLevelName { get; set; }

        public virtual int AuthLevelValue
        {
            get { return (int)AuthLevelID; }
        }

        public static IEnumerable<string> AuthLevelText(ClientAuthLevel level)
        {
            foreach (Enum value in Enum.GetValues(typeof(ClientAuthLevel)))
                if (level.HasFlag(value))
                    yield return Enum.GetName(typeof(ClientAuthLevel), value);
        }
    }
}
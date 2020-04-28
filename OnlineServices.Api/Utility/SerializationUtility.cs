using LNF.Util.Serialization;
using System;

namespace OnlineServices.Api.Utility
{
    public class SerializationUtility : ISerializationUtility
    {
        public ISerializer Xml => throw new NotImplementedException();

        public ISerializer Json => throw new NotImplementedException();
    }
}

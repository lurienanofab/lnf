using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LNF.GoogleApi
{
    public class UserInfo
    {
        [JsonProperty("id")]
        public string ID { get; set; }
        
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("name")]
        public UserName Name { get; set; }

        [JsonProperty("emails")]
        public UserEmail[] Emails { get; set; }

        [JsonProperty("image")]
        public UserImage Image { get; set; }

        [JsonProperty("domain")]
        public string Domain { get; set; }
    }

    public class UserEmail
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public class UserName
    {
        [JsonProperty("familyName")]
        public string FamilyName { get; set; }

        [JsonProperty("givenName")]
        public string GivenName { get; set; }
    }

    public class UserImage
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("isDefault")]
        public bool IsDefault { get; set; }
    }
}

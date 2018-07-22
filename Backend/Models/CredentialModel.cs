using Newtonsoft.Json;
using System;

namespace Backend.Models
{
    public class CredentialModel
    {
        [JsonProperty(PropertyName = "id")]
        public Guid UserId { get; set; }

        [JsonProperty(PropertyName = "userName")]
        public string UserName { get; set; }

        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }

        [JsonProperty(PropertyName = "salt")]
        public string Salt { get; set; }
    }
}

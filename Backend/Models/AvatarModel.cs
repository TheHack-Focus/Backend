using Newtonsoft.Json;

namespace Backend.Models
{
    public class AvatarModel
    {
        [JsonProperty(PropertyName = "id")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "avatarLink")]
        public string AvatarLink { get; set; }
    }
}

using Backend.Utils;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class CardModel
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "timeStamp")]
        [Required]
        public DateTimeOffset TimeStamp { get; set; }

        [JsonProperty(PropertyName = "publisher")]
        public PublisherModel Publisher { get; set; }

        [JsonProperty(PropertyName = "location")]
        [Required]
        public LocationModel Location { get; set; }

        [JsonProperty(PropertyName = "title")]
        [Required]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "media")]
        public MediaModel Media { get; set; }
    }

    public class PublisherModel
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
    }

    public class MediaModel
    {
        public string[] Images { get; set; }
    }

    public enum MediaType
    {
        Images,
        Videos,
        LinkCards
    }

    public class LinkCardModel
    {
        public string Title { get; set; }
        public string Preview { get; set; }
        public string Url { get; set; }
    }

    public class LocationModel
    {
        [LatValidation]
        public double Lat { get; set; }
        [LonValidation]
        public double Lon { get; set; }
        public string Description { get; set; }
    }
}

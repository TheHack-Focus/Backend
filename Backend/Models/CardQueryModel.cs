using Backend.Utils;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class CardQueryModel
    {
        [JsonIgnore]
        public const double DefaultRadius = 100.00;

        [LatValidation]
        [Required]
        public double Lat { get; set; }

        [LonValidation]
        [Required]
        public double Lon { get; set; }

        public double Radius { get; set; }

        public DateTimeOffset Epoch { get; set; }
    }
}

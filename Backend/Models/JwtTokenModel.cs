using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class JwtTokenModel
    {
        [Required]
        public string Token { get; set; }

        public JwtTokenModel() { }
        public JwtTokenModel(string token)
        {
            Token = token;
        }
    }
}

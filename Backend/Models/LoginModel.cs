using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class LoginModel
    {
        public Guid Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }

    public class TokenModel
    {
        public string Token { get; set; }
    }
}

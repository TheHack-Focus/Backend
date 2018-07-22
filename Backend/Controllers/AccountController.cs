using Backend.DataObjects;
using Backend.Models;
using Backend.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    public class AccountController : Controller
    {
        private IdentityDocumentDBRepository<CredentialModel> _credStorage;
        private AvatarDocumentDBRepository<AvatarModel> _avatarStorage;

        private IOptions<JwtConfig> _jwtConfig;

        public AccountController(
            IdentityDocumentDBRepository<CredentialModel> credStorage,
            AvatarDocumentDBRepository<AvatarModel> avatarStorage,
            IOptions<JwtConfig> jwtConfig)
        {
            _credStorage = credStorage;
            _avatarStorage = avatarStorage;
            _jwtConfig = jwtConfig;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            Guid userId;
            string userName;

            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorMessageModel("Required parameter(s) are not present."));
            }

            // Check if this user exists in database.
            var users = await _credStorage.GetItemsAsync(k => k.UserName == loginModel.Username);
            if (users.Any())
            {
                var dbUser = users.First();
                if (!SecurityUtils.ValidatePassword(loginModel.Password, dbUser.Salt, dbUser.Password))
                {
                    return BadRequest(new ErrorMessageModel("Invalid credential."));
                }

                // Otherwise validation succeeded
                userId = dbUser.UserId;
                userName = dbUser.UserName;
            }
            else
            {
                // Create user
                userId = Guid.NewGuid();
                userName = loginModel.Username;
                var salt = SecurityUtils.GenerateSalt();

                await _credStorage.CreateItemAsync(new CredentialModel
                {
                    UserId = userId,
                    UserName = loginModel.Username,
                    Password = SecurityUtils.CreatePasswordHash(loginModel.Password, salt),
                    Salt = salt
                });
            }

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Value.SigningToken));
            var signingCredentials = new SigningCredentials(signingKey,
                SecurityAlgorithms.HmacSha256Signature, SecurityAlgorithms.Sha256Digest);

            var claimsIdentity = new ClaimsIdentity(new List<Claim>()
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            }, JwtBearerDefaults.AuthenticationScheme);

            var securityTokenDescriptor = new SecurityTokenDescriptor()
            {
                Audience = _jwtConfig.Value.Audience,
                Issuer = _jwtConfig.Value.Issuer,
                Subject = claimsIdentity,
                SigningCredentials = signingCredentials,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var plainToken = tokenHandler.CreateToken(securityTokenDescriptor);
            var signedAndEncodedToken = tokenHandler.WriteToken(plainToken);

            return Ok(new JwtTokenModel(signedAndEncodedToken));
        }

        [HttpGet]
        public async Task<IActionResult> Avatar(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var entry = await _avatarStorage.GetItemAsync(id);
                if (entry != null)
                {
                    return Redirect(entry.AvatarLink);
                }
            }

            // Fallback
            return Redirect("https://hackshanghai.blob.core.windows.net/avatar/DefaultAvatar.PNG");
        }
    }
}

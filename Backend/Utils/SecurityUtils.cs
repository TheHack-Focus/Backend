using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Backend.Utils
{
    public static class SecurityUtils
    {
        public static string GenerateSalt()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] rngBuffer = new byte[64];
                rng.GetBytes(rngBuffer, 0, 64);

                return Convert.ToBase64String(rngBuffer);
            }
        }

        public static string CreatePasswordHash(string value, string salt)
        {
            var valueBytes = KeyDerivation.Pbkdf2(
                                password: value,
                                salt: Encoding.UTF8.GetBytes(salt),
                                prf: KeyDerivationPrf.HMACSHA512,
                                iterationCount: 10000,
                                numBytesRequested: 256 / 8);

            return Convert.ToBase64String(valueBytes);
        }

        public static bool ValidatePassword(string value, string salt, string hash)
            => CreatePasswordHash(value, salt) == hash;
    }
}

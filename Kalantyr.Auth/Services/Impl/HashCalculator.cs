using System;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Kalantyr.Auth.Services.Impl
{
    public class HashCalculator: IHashCalculator
    {
        /// <inheritdoc/>
        public string GetHash(string password, string salt)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(password, Convert.FromBase64String(salt), KeyDerivationPrf.HMACSHA256, 1000, 256 / 8));
        }
    }
}

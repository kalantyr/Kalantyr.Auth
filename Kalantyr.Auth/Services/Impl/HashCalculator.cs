using System;
using System.IO;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Kalantyr.Auth.Services.Impl
{
    public class HashCalculator: IHashCalculator
    {
        /// <inheritdoc/>
        public string GetHash(string password, string salt)
        {
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentNullException(nameof(password));
            if (string.IsNullOrWhiteSpace(salt)) throw new ArgumentNullException(nameof(salt));

            return GetHashImpl(password, salt);
        }

        internal static string GetHashImpl(string password, string salt)
        {
            var saltBinary = ToBinary(salt);
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(password, saltBinary, KeyDerivationPrf.HMACSHA256, 1000, 256 / 8));
        }

        internal static byte[] ToBinary(string value)
        {
            using var stream = new MemoryStream();
            using var writer = new StreamWriter(stream);
            writer.Write(value);
            writer.Flush();
            var saltBinary = stream.ToArray();
            return saltBinary;
        }
    }
}

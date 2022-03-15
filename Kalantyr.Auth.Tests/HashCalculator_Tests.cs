using Kalantyr.Auth.Services.Impl;
using NUnit.Framework;

namespace Kalantyr.Auth.Tests
{
    public class HashCalculator_Tests
    {
        [TestCase("qwerty", "ABCD")]
        [TestCase("admin123", "0123456789")]
        public void GetHash_Test(string password, string salt)
        {
            var hashCalculator = new HashCalculator();

            var hash1 = hashCalculator.GetHash(password, salt);
            var hash2 = hashCalculator.GetHash(password, salt);
            Assert.AreEqual(hash1, hash2);

            hash1 = hashCalculator.GetHash(password, salt);
            hash2 = hashCalculator.GetHash(password + "2", salt);
            Assert.AreNotEqual(hash1, hash2);

            hash1 = hashCalculator.GetHash(password, salt);
            hash2 = hashCalculator.GetHash(password, salt + "2");
            Assert.AreNotEqual(hash1, hash2);
        }
    }
}

namespace Kalantyr.Auth.Services
{
    public interface IHashCalculator
    {
        string GetHash(string password, string salt);
    }
}

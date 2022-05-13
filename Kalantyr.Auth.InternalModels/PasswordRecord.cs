namespace Kalantyr.Auth.InternalModels
{
    public class PasswordRecord
    {
        public uint UserId { get; set; }

        public string PasswordHash { get; set; }

        public string Salt { get; set; }
    }
}

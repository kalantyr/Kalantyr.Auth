namespace Kalantyr.Auth.Models
{
    public class UserRecord
    {
        public uint Id { get; set; }

        public string Login { get; set; }

        public string PasswordHash { get; set; }

        public string Salt { get; set; }
    }
}

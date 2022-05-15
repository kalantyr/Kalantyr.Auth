namespace Kalantyr.Auth.DbRepositories.Entities
{
    public class Password
    {
        public uint Id { get; set; }

        public uint UserId { get; set; }

        public User User { get; set; }

        public string PasswordHash { get; set; }

        public string Salt { get; set; }
    }
}

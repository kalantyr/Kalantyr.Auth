using System.ComponentModel.DataAnnotations;

namespace Kalantyr.Auth.DbRepositories.Entities
{
    public class Password
    {
        public uint Id { get; set; }

        public uint UserId { get; set; }

        public User User { get; set; }

        [MaxLength(128)]
        public string PasswordHash { get; set; }

        [MaxLength(64)]
        public string Salt { get; set; }
    }
}

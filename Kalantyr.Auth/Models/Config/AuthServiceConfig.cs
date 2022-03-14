using System;

namespace Kalantyr.Auth.Models.Config
{
    public class AuthServiceConfig
    {
        public TimeSpan TokenLifetime { get; set; } = TimeSpan.FromHours(1);
    }
}

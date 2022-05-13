using System;
using Kalantyr.Auth.InternalModels;

namespace Kalantyr.Auth.Models.Config
{
    public class AuthServiceConfig
    {
        public TimeSpan TokenLifetime { get; set; } = TimeSpan.FromHours(1);

        public UserRecord[] Users { get; set; } = Array.Empty<UserRecord>();

        public PasswordRecord[] Passwords { get; set; } = Array.Empty<PasswordRecord>();

        public AppKeyConfig[] AppKeys { get; set; } = Array.Empty<AppKeyConfig>();

        public class AppKeyConfig
        {
            public string AppName { get; set; }

            public string Key { get; set; }
        }
    }
}

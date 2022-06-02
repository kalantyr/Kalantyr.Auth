using System.Collections.Generic;

namespace Kalantyr.Auth.AdminTool
{
    internal class Config
    {
        public static Config Instance { get; } = new();

        private Config() { }

        public IReadOnlyCollection<Environment> Environments { get; } = new []
        {
            new Environment
            {
                AuthApiUrl = "http://WIN-QNU9TO285O8/auth"
            }
        };
    }
}

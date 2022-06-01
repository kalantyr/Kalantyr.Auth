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
                AuthApiUrl = "http://win-qnu9to285o8/auth"
            }
        };
    }
}

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
                DbConnectionString = "Data Source=XX.XX.XX.XX;Initial Catalog=XXXXXXXX;User ID=XXXXXXXX;Password=XXXXXX;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"
            }
        };
    }
}

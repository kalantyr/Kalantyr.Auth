using System;

namespace Kalantyr.Auth.Models
{
    public class TokenInfo
    {
        public string Value { get; set; }

        public DateTimeOffset ExpirationDate { get; set; }
    }
}

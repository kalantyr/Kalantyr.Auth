using System;
using Kalantyr.Auth.DbRepositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kalantyr.Auth.DbRepositories
{
    internal class AuthDbContext: DbContext
    {
        private readonly string _connectionString;

        public AuthDbContext()
        {
        }

        public AuthDbContext(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
            {
                if (string.IsNullOrEmpty(_connectionString))
                    options.UseSqlServer();
                else
                    options.UseSqlServer(_connectionString);
            }

            base.OnConfiguring(options);
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Password> Passwords { get; set; }
    }
}

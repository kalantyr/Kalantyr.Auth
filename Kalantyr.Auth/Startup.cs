using System;
using Kalantyr.Auth.DbRepositories;
using Kalantyr.Auth.InternalModels;
using Kalantyr.Auth.Models.Config;
using Kalantyr.Auth.Services;
using Kalantyr.Auth.Services.Impl;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Kalantyr.Auth
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AuthServiceConfig>(_configuration.GetSection("AuthService"));

            services.AddScoped<IUserStorageAdmin, UserStorage>();
            services.AddScoped<IAdminService, AdminService>();

            services.AddScoped<IUserStorage>(sp => new CombinedUserStorage(
                sp.GetService<IOptions<AuthServiceConfig>>(),
                new UserStorage(_configuration)));
            services.AddScoped<IUserStorageReadonly>(sp => sp.GetService<IUserStorage>());
            services.AddScoped<IHashCalculator, HashCalculator>();
            services.AddSingleton<ITokenStorage, TokenStorage>();
            services.AddScoped<ILoginValidator, LoginValidator>();
            services.AddScoped<IPasswordValidator, PasswordValidator>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IHealthCheck, AuthService>();

            services.AddSwaggerDocument();

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseRouting();
            app.UseEndpoints(routeBuilder => routeBuilder.MapControllers());
        }
    }
}

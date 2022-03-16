using System;
using Kalantyr.Auth.Models.Config;
using Kalantyr.Auth.Services;
using Kalantyr.Auth.Services.Impl;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

            services.AddSingleton<IUserStorage>(sp => new UserStorage(sp.GetService<IOptions<AuthServiceConfig>>()));
            services.AddSingleton<IHashCalculator>(new HashCalculator());
            services.AddSingleton<ITokenStorage>(sp => new TokenStorage());
            services.AddSingleton<IAuthService>(sp => new AuthService(
                sp.GetService<IUserStorage>(),
                sp.GetService<IHashCalculator>(),
                sp.GetService<ITokenStorage>(),
                sp.GetService<IOptions<AuthServiceConfig>>()));

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseRouting();
            app.UseEndpoints(routeBuilder => routeBuilder.MapControllers());
        }
    }
}

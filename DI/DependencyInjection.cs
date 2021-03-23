using Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using HttpSocket;
using System.Net.Http;
using Docker;

namespace DI
{
    public static class DependencyInjection
    {
        public static void AddServiceHandle(this IServiceCollection services)
        {
            services.AddScoped<HttpClient>(x => UnixHttpClient.CreateHttpClient("/var/run/docker.sock"));
            services.AddScoped<IServiceHandle, DockerServiceHandle>();
            services.AddScoped<IImageHandle, DockerImageHandle>();
        }

        public static void AddTokenParser(this IServiceCollection services)
        {
            services.AddAuthentication()
                    .AddCookie(options => {
                        options.LoginPath = "/Account/Unauthorized/";
                        options.AccessDeniedPath = "/Account/Forbidden/";
                    })
                    .AddJwtBearer(options => {
                        options.Audience = "http://localhost:5001/";
                        options.Authority = "http://localhost:5000/";
                        options.RequireHttpsMetadata = false;
                        options.Validate("test");
                    });

            services.AddAuthorization();
        }

    }
}
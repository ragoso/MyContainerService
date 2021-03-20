using Core;
using Docker;
using HttpSocket;
using Microsoft.Extensions.DependencyInjection;

namespace Endpoint
{
    public static class DependencyInjection
    {
        public static void DefineServiceHandle(this IServiceCollection services)
        {
            services.AddScoped<IServiceHandle, DockerServiceHandle>(x => new DockerServiceHandle(new UnixHttpClient("/var/run/docker.sock").HttpClient));
        }
    }
}
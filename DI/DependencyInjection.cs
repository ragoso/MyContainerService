using Core;
using DI;
using Docker;
using HttpSocket;
using Microsoft.Extensions.DependencyInjection;

namespace DI
{
    public static class DependencyInjection
    {
        public static void DefineServiceHandle(this IServiceCollection services)
        {
            services.AddScoped<IServiceHandle>(x => ServiceHandleFactory.CreateServiceHandle());
        }
    }
}
using System;
using System.Linq;
using System.Threading;
using Core;
using Docker;
using HttpSocket;

namespace Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var httpClient = new UnixHttpClient("/var/run/docker.sock").HttpClient;

            IServiceHandle handle = new DockerServiceHandle(httpClient);

            var services = handle.GetServices().Result;

            for(var i = 1; i < 500; i++)
            {
                handle.CreateService(new Core.DTO.MyService($"teste{i}", "redis"));
            }
            
            do
            {
                services = handle.GetServices().Result;
                handle.RemoveService(services.FirstOrDefault().Id);
            }
            while(services.Any());

            services = handle.GetServices().Result;
        }
    }
}

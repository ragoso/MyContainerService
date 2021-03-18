using System;
using System.Linq;
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
            var services = handle.GetServices().ToList();
        }
    }
}

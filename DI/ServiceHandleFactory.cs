using Core;
using Docker;
using HttpSocket;

namespace DI
{
    public class ServiceHandleFactory
    {
        public static IServiceHandle CreateServiceHandle()
        {
            return new DockerServiceHandle(UnixHttpClient.CreateHttpClient("/var/run/docker.sock"));
        }
    }
}
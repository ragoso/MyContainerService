using System;
using Core;
using Docker;
namespace Console
{
    class Program
    {
        static void Main(string[] args)
        {

            IServiceHandle handle = new DockerServiceHandle();
            handle.GetServices();
        }
    }
}

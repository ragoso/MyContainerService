using System;
using System.Linq;
using System.Threading;
using Core;
using Docker;
using Endpoint;
using Grpc.Net.Client;
using HttpSocket;

namespace Console
{
    class Program
    {
        static void Main(string[] args)
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client =  new MyContainerService.MyContainerServiceClient(channel);
            var reply = client.Update(new UpdateRequest(){
                Service = new Service()
                {
                    // Mount obj
                }
            });
            
        }
    }
}

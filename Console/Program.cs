using Endpoint;
using Core.DTO;
using Grpc.Net.Client;
using System.Linq;
using System;
using System.Net.Http;

namespace Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new MyService("teste", "redis");
            



            var httpHandler = new HttpClientHandler();
            // Return `true` to allow certificates that are untrusted/invalid
            httpHandler.ServerCertificateCustomValidationCallback = 
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            var channel = GrpcChannel.ForAddress("https://localhost:5001",
            new GrpcChannelOptions { HttpHandler = httpHandler });
            

            var client =  new MyContainerService.MyContainerServiceClient(channel);

            var reply = client.Create(new CreateRequest()
            {
                Service = service.ToGrpcService()
            });

            System.Console.WriteLine(reply.Message);

            var replyGet = client.Get(new GetRequest());

            replyGet.Services.FromGrpcService().ToList().ForEach(x => System.Console.WriteLine($"{x.Id} - {x.Name}"));
            
            client.Remove(new RemoveRequest()
            {
                ServiceNameOrId = "teste"
            });
        }
    }
}

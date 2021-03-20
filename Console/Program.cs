using Endpoint;
using Core.DTO;
using Grpc.Net.Client;
using System.Linq;
using System;
using System.Net.Http;
using System.IO;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Console
{
    class Program
    {
        static void Main(string[] args)
        {

            try
            {
                var url = GetParam(args, "url");
                var serviceJsonParam = GetParam(args, "service-json");
                var action = GetParam(args, "action");

                var service = ReadMyServiceJson(serviceJsonParam);

                var client = GetClient(url);

                switch(action)
                {
                    case "create":
                        Create(service, client);
                        break;
                    case "remove":
                        Remove(service, client);
                        break;
                    case "update":
                        Update(service, client);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("Invalid action informed.");
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }

        }

        private static void Update(MyService service, MyContainerService.MyContainerServiceClient client)
        {
            var reply = client.Update(new UpdateRequest()
            {
                Service = service.ToGrpcService()
            });

            System.Console.WriteLine(reply.Message);
        }

        private static void Remove(MyService service, MyContainerService.MyContainerServiceClient client)
        {
            var reply = client.Remove(new RemoveRequest() {
                ServiceNameOrId = service.Name
            });

            System.Console.WriteLine(reply.Message);
        }

        private static void Create(MyService service, MyContainerService.MyContainerServiceClient client)
        {
            var reply = client.Create(new CreateRequest()
            {
                Service = service.ToGrpcService()
            });
            System.Console.WriteLine(reply.Message);
        }

        private static void PrintHelp()
        {
            System.Console.WriteLine("--url=https://myEndpointGrpd");
            System.Console.WriteLine("--service-json=./myservice.json");
            System.Console.WriteLine("--action=create|remove|update");
            System.Console.WriteLine();
            PrintJsonExample();
        }

        private static void PrintJsonExample()
        {
            var service = new MyService("test", "redis", "backend", "db_net")
            {
                Id = Guid.NewGuid().ToString(),
                Labels = new Dictionary<string, string>()
                {
                    {"createdByMyService", "true"}
                },
                Volumes = new List<Core.DTO.Volume>()
                {
                    new Core.DTO.Volume(false, "/tmp/data", "/app")
                }
            };

            System.Console.WriteLine(JsonConvert.SerializeObject(service));
        }

        private static string GetParam(string[] args, string paramName)
        {
            if (args.Contains("--help") || args.Contains("-h") || args.Length == 0)
            {
                PrintHelp();
                Environment.Exit(0);
            }

            var param = args.FirstOrDefault(x => x.Contains($"--{paramName}="));

            if (string.IsNullOrEmpty(param))
            {
                throw new ArgumentNullException($"Argument --{paramName}= must be passed.");
            }

            var paramSplited = param.Split("=");

            if (paramSplited.Length != 2)
            {
                throw new ArgumentNullException($"Argument --{paramName}= has passed with invalid format");
            }

            return paramSplited.Last();
        }

        private static MyContainerService.MyContainerServiceClient GetClient(string url)
        {
            var httpHandler = new HttpClientHandler();
            // Return `true` to allow certificates that are untrusted/invalid
            httpHandler.ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            var channel = GrpcChannel.ForAddress(url,
            new GrpcChannelOptions { HttpHandler = httpHandler });

            var client = new MyContainerService.MyContainerServiceClient(channel);
            return client;
        }

        private static MyService ReadMyServiceJson(string jsonPath)
        {
            var json = File.ReadAllText(jsonPath);

            return JsonConvert.DeserializeObject<MyService>(json);
        }
    }
}

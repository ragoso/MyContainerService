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
using Mono.Options;
using Grpc.Core;

namespace Console
{
    public enum Actions
    {
        Create,
        Update,
        Remove,
        List,
        Non
    }
    class Program
    {
        private static string url = "https://localhost:5001";
        private static string json = string.Empty;
        private static Actions action = Actions.Non;
        private static string token = string.Empty;

        static void Main(string[] args)
        {

            try
            {
                ParserAgrs(args);

                var service = ReadMyServiceJson(json);

                var client = GetClient(url);

                switch(action)
                {
                    case Actions.Create:
                        Create(service, client);
                        break;
                    case Actions.Remove:
                        Remove(service, client);
                        break;
                    case Actions.Update:
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

        private static void ParserAgrs(IEnumerable<string> args)
        {
            var showHelp = false;
            var writeJson = false;
            var opt = new OptionSet()
            {
                {"u|url=", "The url of grpc endpoints", u => url = u },
                {"k|key=", "The path of key SSL/TLS credentials", u => token = u},
                {"j|json=", "The json of service to be handled", j => json = j},
                {"a|action=", "The action to perform (create,update,remove)", a => action = (Actions)Enum.Parse(typeof(Actions), a)},
                {"w|write", "Write json example", w => writeJson = w != null},
                {"h|help", "Print help", h => showHelp = h != null}
            };

            try
            {
                opt.Parse(args);
            }
            catch (OptionException e)
            {
                System.Console.WriteLine (e.Message);
                System.Console.WriteLine ("Try `Console --help' for more information.");
                return;
            }

            if (showHelp)
            {
                ShowHelp(opt);
                Environment.Exit(0);
            }

            if (writeJson)
            {
                PrintJsonExample();
                Environment.Exit(0);       
            }

            if (string.IsNullOrEmpty(token))
            {
                System.Console.WriteLine("Key path must be passwd with -k or --key");
                Environment.Exit(1);
            }

            if (string.IsNullOrEmpty(json))
            {
                System.Console.WriteLine("Json path must be passed with -j or --json");
                Environment.Exit(1);
            }
            
        }
        private static void ShowHelp(OptionSet opt)
        {
            System.Console.WriteLine("Usage: Console ...");
            System.Console.WriteLine("Interact com MyContainerService");
            System.Console.WriteLine();
            System.Console.WriteLine("Options:");
            opt.WriteOptionDescriptions (System.Console.Out);
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

        private static MyContainerService.MyContainerServiceClient GetClient(string url)
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            var channel = GrpcChannel.ForAddress(url,
            new GrpcChannelOptions { HttpHandler = httpHandler});

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

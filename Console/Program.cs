using Endpoint;
using Core.DTO;
using Grpc.Net.Client;
using System;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using Mono.Options;
using Grpc.Core;
using GRPC;
using Core;

namespace Console
{
    public enum Actions
    {
        Create,
        Update,
        Remove,
        List,
        Build,
        Non
    }
    class Program
    {
        private static string url = "https://localhost:5001";
        private static string file = string.Empty;
        private static Actions action = Actions.Non;
        private static string token = string.Empty;
        private static string tag = "latest";

        private static IServiceHandle _serviceHandle;
        private static IImageHandle _imageHandle;

        static void Main(string[] args)
        {

            try
            {
                ParserArgs(args);

                switch(action)
                {
                    case Actions.Create:
                        Create();
                        break;
                    case Actions.Remove:
                        Remove();
                        break;
                    case Actions.Update:
                        Update();
                        break;
                    case Actions.Build:
                        BuildImage();
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
        private static void BuildImage()
        {
            var client = GetImageClient(url);

            _imageHandle = new ImageClientHandle(GetImageClient(url), token);

            using var fileStream = new FileStream(file, FileMode.Open);

            var reply = _imageHandle.BuildImage(fileStream, tag).Result;

            System.Console.WriteLine(reply);

        }
        private static void Update()
        {
            
            var service = ReadMyServiceJson(file);

            _serviceHandle = new ServiceClientHandle(GetServiceClient(url), token);

           var reply = _serviceHandle.UpdateService(service).Result;

            System.Console.WriteLine(reply);
        }

        private static void Remove()
        {
            var service = ReadMyServiceJson(file);

            _serviceHandle = new ServiceClientHandle(GetServiceClient(url), token);

            var reply = _serviceHandle.RemoveService(service.Id ?? service.Name).Result;

            System.Console.WriteLine(reply);
        }

        private static void Create()
        {
            var service = ReadMyServiceJson(file);

            _serviceHandle = new ServiceClientHandle(GetServiceClient(url), token);

            var response = _serviceHandle.CreateService(service, true).Result;

            System.Console.WriteLine(response);
        }

        private static void ParserArgs(IEnumerable<string> args)
        {
            var showHelp = false;
            var writeJson = false;
            var opt = new OptionSet()
            {
                {"u|url=", "The url of grpc endpoints", u => url = u },
                {"f|file=", "The json of service or tar of image to be handled", j => file = j},
                {"t|tag=", "The image tag to build. If not passed, latest be default.", t => tag = t },
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

            if (string.IsNullOrEmpty(file))
            {
                System.Console.WriteLine("File path must be passed with -f or --f");
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
                },
                Ports = new List<Core.DTO.Port>()
                {
                    new Core.DTO.Port(80, "tcp")
                }
            };

            System.Console.WriteLine(JsonConvert.SerializeObject(service));
        }

        private static MyContainerService.MyContainerServiceClient GetServiceClient(string url)
        {
            GrpcChannel channel = CreateChannel(url);

            var client = new MyContainerService.MyContainerServiceClient(channel);
            return client;
        }

        private static GrpcChannel CreateChannel(string url)
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            var channel = GrpcChannel.ForAddress(url,
            new GrpcChannelOptions { HttpHandler = httpHandler });
            return channel;
        }

        private static MyContainerImage.MyContainerImageClient GetImageClient(string url)
        {
            GrpcChannel channel = CreateChannel(url);

            var client = new MyContainerImage.MyContainerImageClient(channel);
            return client;
        }

        private static MyService ReadMyServiceJson(string jsonPath)
        {
            var json = File.ReadAllText(jsonPath);

            return JsonConvert.DeserializeObject<MyService>(json);
        }
    }
}

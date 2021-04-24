using Endpoint;
using Core.DTO;
using Grpc.Net.Client;
using System;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using Mono.Options;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Core;
using System.Linq;

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
        private static bool useStream = false;
        private static int bufferLength = 4 * 1024;
        private static Actions action = Actions.Non;
        private static string token = string.Empty;
        private static string tag = string.Empty;
        private static IList<string> buildParam = new List<string>();
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
            if (!File.Exists(file))
            {
                throw new FileNotFoundException();
            }

            if (string.IsNullOrEmpty(tag))
            {
                throw new ArgumentNullException("tag");
            }
            
            _imageHandle = new ImageClientHandle(GetImageClient(url), token, bufferLength);

            

            string reply;
            if (useStream)
            {
                var filesStream = File.OpenRead(file);
                
                reply = _imageHandle.BuildImage(filesStream, buildParam, tag).Result;
            }
            else
            {
                var fileBytes = File.ReadAllBytes(file);
                reply = _imageHandle.BuildImage(fileBytes, buildParam, tag).Result;
            }

            System.Console.WriteLine(reply);

        }
        private static void Update()
        {   
            if (string.IsNullOrEmpty(tag))
            {
                throw new ArgumentNullException("tag");
            }

            _serviceHandle = new ServiceClientHandle(GetServiceClient(url), token);

            var services = ReadMyServiceFile();

            services.ToList().ForEach(x => {

                if(string.IsNullOrEmpty(x.Image)) x.Image = tag;
                
                var reply = _serviceHandle.UpdateService(x).Result;

                System.Console.WriteLine(reply);
            });
        }

        private static void Remove()
        {
            var services = ReadMyServiceFile();

            _serviceHandle = new ServiceClientHandle(GetServiceClient(url), token);

            services.ToList().ForEach(x => 
            {
                var reply = _serviceHandle.RemoveService(x.Id ?? x.Name).Result;
                 System.Console.WriteLine(reply);
            });
        }

        private static void Create()
        {
             var services = ReadMyServiceFile();

            _serviceHandle = new ServiceClientHandle(GetServiceClient(url), token);

            services.ToList().ForEach(x => {

                var response = _serviceHandle.CreateService(x, true).Result;

                System.Console.WriteLine(response);
            });
        }

        private static void ParserArgs(IEnumerable<string> args)
        {
            var showHelp = false;
            var writeYaml = false;
            var opt = new OptionSet()
            {
                {"u|url=", "The url of grpc endpoints", u => url = u },
                {"f|file=", "The yaml or json of service or tar of image to be handled", j => file = j},
                {"t|tag=", "The image tag to build and update.", t => tag = t },
                {"p|param=", "The param to build. Ex: foo=bar", p => buildParam.Add(p) },
                {"s|stream", "Use streaming transfer to build images.", p => useStream = p != null},
                {"b|buffer=", "The buffer size to streaming image build", b => bufferLength = int.Parse(b)},
                {"a|action=", "The action to perform (create,update,remove)", a => action = (Actions)Enum.Parse(typeof(Actions), a)},
                {"w|write", "Write yaml example", w => writeYaml = w != null},
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

            if (writeYaml)
            {
                PrintYamlExample();
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
            MyService service = MountObjExample();

            System.Console.WriteLine(JsonConvert.SerializeObject(service));
        }

        private static void PrintYamlExample()
        {
            var service = MountObjExample();

            var serializer = new SerializerBuilder()
                                //.WithNamingConvention(UnderscoredNamingConvention.Instance)
                                .Build();

            System.Console.WriteLine(serializer.Serialize(new List<MyService>() { service }));
        }

        private static MyService MountObjExample()
        {
            return new MyService("test", "redis", "backend", "db_net")
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

        private static IList<MyService> ReadMyServiceFile()
        {
            var fileInfo = new FileInfo(file);

            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException();
            }

            if (fileInfo.Extension.Equals(".json"))
            {
                return ReadMyServiceJson(file);
            }

            if (fileInfo.Extension.Equals(".yaml"))
            {
                return ReadMyServiceAsYaml(file);
            }

            throw new FileLoadException();
        }

        private static IList<MyService> ReadMyServiceAsYaml(string yamlPath)
        {
            var yamlContent = File.ReadAllText(yamlPath);

            var deserializer = new DeserializerBuilder()
                               // .WithNamingConvention(UnderscoredNamingConvention.Instance)
                                .Build();

            return deserializer.Deserialize<IList<MyService>>(yamlContent);

        }

        private static IList<MyService> ReadMyServiceJson(string jsonPath)
        {
            var json = File.ReadAllText(jsonPath);

            return JsonConvert.DeserializeObject<IList<MyService>>(json);
        }
    }
}

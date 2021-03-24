using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Core.DTO;
using Newtonsoft.Json;

namespace Docker
{
    public static class Extensions
    {
        public static DockerServiceResponse GetDockerService(this MyService myService)
        {
            return new DockerServiceResponse()
            {
                ID = myService.Id,
                Version = new Version(){
                    Index = myService.Version
                },
                Spec = new DockerService()
                    {
                        Name = myService.Name,
                        Labels = myService.Labels,
                        TaskTemplate = new TaskTemplate()
                        {
                            ContainerSpec = new ContainerSpec()
                            {
                                Image = myService.Image,
                                Mounts = myService.Volumes?.Select(x => new Mount()
                                {
                                    ReadOnly = x.ReadOnly,
                                    Source = x?.Source,
                                    Target = x.Target
                                })?.ToList()
                            },
                            Networks = myService.Networks?.Select(x => new Network()
                            {
                                Target = x
                            
                            })?.ToList()
                        },
                        EndpointSpec = new EndpointSpec()
                        {
                            Ports = myService.Ports.Select(x => new Port()
                            {
                                Protocol = x.Protocol,
                                PublishedPort = x.ExternalPort ?? 0,
                                TargetPort = x.InternalPort
                            }).ToList()
                        }
                    }
            };
            
        }

        public static MyService GetMyService(this DockerServiceResponse service)
        {
           return new MyService(service.Name, service.Image, service.Networks?.ToArray())
           {
                Id = service.ID,
                Labels = service.Labels,
                Version = service.Version.Index,
                Volumes = service.Mounts?.Select(x => new Volume(x.ReadOnly, x.Source, x.Target)),
                Ports = service.Ports?.Select(x => new Core.DTO.Port(x.TargetPort, x.PublishedPort, x.Protocol))
           };
        }

        public static IEnumerable<MyService> GetMyServices(this IEnumerable<DockerServiceResponse> containers)
        {
            return containers.Select(x => x.GetMyService());
        }

        public static T FromJson<T>(this string obj)
        {
            return JsonConvert.DeserializeObject<T>(obj);
        }

        public static string AsJson<T>(this T obj)
        {

            return JsonConvert.SerializeObject(obj);
        }

    }
}
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Core.DTO;
using Newtonsoft.Json;

namespace Docker
{
    public static class Extensions
    {
        public static DockerService GetDockerService(this MyService myService)
        {
            return new DockerService()
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
                }
            };
        }

        public static MyService GetMyService(this DockerService dockerService)
        {
            return new MyService(string.Empty, string.Empty);
        }

        public static MyService GetMyService(this DockerServiceResponse service)
        {
           return new MyService(service.Name, service.Image, service.Networks?.ToArray())
           {
                Id = service.ID,
                Labels = service.Labels,
                Volumes = service.Mounts?.Select(x => new Volume(x.ReadOnly, x.Source, x.Target))
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
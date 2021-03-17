using System.Linq;
using Core.DTO;

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
                        Mounts = myService.Volumes.Select(x => new Mount()
                        {
                            ReadOnly = x.ReadOnly,
                            Source = x.Source,
                            Target = x.Target
                        }).ToList()
                    },
                    Networks = myService.Networks.Select(x => new Network()
                    {
                        Target = x
                    }).ToList()
                }
            };
        }

        public static MyService GetMyService(this DockerService dockerService)
        {
            return new MyService(string.Empty, string.Empty);
        }
    }
}
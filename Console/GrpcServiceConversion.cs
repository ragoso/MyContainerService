using System.Collections.Generic;
using System.Linq;
using Core.DTO;
using Endpoint;

namespace Console
{
    public static class GrpcServiceConversion
    {
        public static IEnumerable<MyService> FromGrpcService(this IEnumerable<Service> services)
        {
            return services.Select(x => x.FromGrpcService());
        }

        public static IEnumerable<Service> ToGrpcService(this IEnumerable<MyService> services)
        {
            return services.Select(x => x.ToGrpcService());
        }
        public static MyService FromGrpcService(this Service service)
        {
            return new MyService(service.Name, service.Image)
            {
                Id = service.Id,
                Labels = service.Labels.ToDictionary(x => x.Key, y => y.Value),
                Networks = service.Networks,
                Volumes = service.Volume.Select(x => new Core.DTO.Volume(x.ReadOnly, x.Source, x.Target))
            };
        }
        public static Service ToGrpcService(this MyService service)
        {
            var serviceGrpc = new Service
            {
                Id = service.Id ?? string.Empty,
                Name = service.Name,
                Image = service.Image,
            };

            if (service.Labels?.Any() ?? false)
            {
                serviceGrpc.Labels.AddRange(service.Labels.Select(x => new Label()
                {
                    Key = x.Key,
                    Value = x.Value
                }));
            }

            if (service.Labels?.Any() ?? false)
            {
                serviceGrpc.Volume.AddRange(service.Volumes?.Select(x => new Endpoint.Volume()
                {
                    ReadOnly = x.ReadOnly,
                    Source = x.Source,
                    Target = x.Target
                }));
            }
            
            if (service.Networks?.Any() ?? false)
            {
                serviceGrpc.Networks.AddRange(service.Networks);
            }
            

            return serviceGrpc;
        }
    }
}
using System.Collections.Generic;
using System.Linq;

namespace Core.DTO
{
    public class MyService
    {
        public MyService(string name, string image)
        {
            Name = name;
            Image = image;
        }

        public MyService(string name, string image, params string[] networks) : this(name, image)
        {
            Networks = networks?.ToList();
        }

        public MyService(string name, string image, IList<Volume> volumes) : this(name, image)
        {
            Volumes = volumes;
        }

        public MyService(string name, string image, IList<Volume> volumes, params string[] networks): this(name, image, volumes)
        {
            Networks = networks.ToList();
        }

        public MyService(string name, string image, TraefikConfig traefikConfig) : this(name, image)
        {
            AddTraefikLabels(traefikConfig);
        }

        public MyService()
        {
            
        }

        public string Id { get; set; }
        public string Name { get; init; }
        public string Image { get; init; }
        public IEnumerable<string> Networks { get; init; }
        public IDictionary<string, string> Labels { get; set; }
        public IEnumerable<Volume> Volumes { get; set; }

        private void AddTraefikLabels(TraefikConfig config)
        {
            if (Labels is null)
            {
                Labels = new Dictionary<string, string>()
                {
                    {"traefik.enable", "true"},
                    {$"traefik.http.services.{Name}.loadbalancer.server.port", config.InternalPort.ToString()},
                    {$"traefik.http.routers.{Name}.rule", $"Host({config.Domain})"},
                    {$"traefik.http.routers.{Name}.entrypoints", config.HttpEntrypoint},
                    {$"traefik.http.middlewares.{Name}.redirectscheme.scheme", "https"},
                    {$"traefik.http.routers.{Name}.middlewares", Name},
                    {$"traefik.http.routers.{Name}-secured.rule", $"Host({config.Domain})"},
                    {$"traefik.http.routers.{Name}-secured.entrypoints", config.HttpsEntrypoint},
                    {$"traefik.http.routers.{Name}-secured.tls", "true"},
                    {$"traefik.http.routers.{Name}-secured.tls.certresolver", config.CertResolver},
                    {$"traefik.docker.network", config.Network}
                };
            }
        }

    }

    public class Volume
    {
        public Volume(bool readOnly, string source, string target)
        {
            ReadOnly = readOnly;
            Source = source;
            Target = target;
        }

        public bool ReadOnly { get; set; }
        public string Source { get; set; }
        public string Target { get; set; }
    }

    public class TraefikConfig
    {
        public string Domain { get; init; }
        public int InternalPort { get; init; }
        public string HttpEntrypoint { get; init; }
        public string HttpsEntrypoint { get; init; }
        public string CertResolver { get; init; }
        public string Network { get; init; }
    }
}
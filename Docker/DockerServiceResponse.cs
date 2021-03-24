using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Docker
{
    public class DockerServiceResponse    
    {
        public DockerService Spec { get; set; }
        public string ID { get; set; }
        [JsonProperty("Version.Index")]
        public int Version {get; set;}
        public string Image => Spec.TaskTemplate.ContainerSpec.Image;
        public string Name => Spec.Name;
        [JsonIgnore]
        public IEnumerable<string> Networks => Spec.TaskTemplate.Networks?.Select(x => x.Target);
        [JsonIgnore]
        public IDictionary<string, string> Labels => Spec.Labels;
        [JsonIgnore]
        public IEnumerable<Mount> Mounts => Spec.TaskTemplate.ContainerSpec.Mounts;
        [JsonIgnore]
        public IEnumerable<Port> Ports => Spec.EndpointSpec.Ports;
        

    }
}
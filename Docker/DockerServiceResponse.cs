using System.Collections.Generic;
using System.Linq;

namespace Docker
{
    public class DockerServiceResponse    
    {
        public DockerService Spec { get; set; }
        public string ID { get; set; }
        public int Version {get; set;}
        public string Image => Spec.TaskTemplate.ContainerSpec.Image;
        public string Name => Spec.Name;
        public IEnumerable<string> Networks => Spec.TaskTemplate.Networks?.Select(x => x.Target);
        public IDictionary<string, string> Labels => Spec.Labels;
        public IEnumerable<Mount> Mounts => Spec.TaskTemplate.ContainerSpec.Mounts;
        public IEnumerable<Port> Ports => Spec.EndpointSpec.Ports;
        

    }
}
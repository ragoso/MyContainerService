using System.Collections.Generic;
using System.Linq;

namespace Docker
{
    public class DockerServiceResponse    
    {
        public DockerService Spec { get; set; }
        public string ID { get; set; }
        public string Image => Spec.TaskTemplate.ContainerSpec.Image;
        public string Name => Spec.Name;
        public IEnumerable<string> Networks => Spec.TaskTemplate.Networks?.Select(x => x.Target);
        public IDictionary<string, string> Labels => Spec.Labels;
        public IList<Mount> Mounts => Spec.TaskTemplate.ContainerSpec.Mounts;
        

    }
}
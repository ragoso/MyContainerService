using System.Collections.Generic;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.Linq;
using System;

namespace Docker
{
   // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 

    public class DriverConfig
    {
    }

    public class Labels
    {
        [JsonProperty("com.example.something")]
        public string ComExampleSomething { get; set; }
        public string foo { get; set; }
    }

    public class VolumeOptions
    {
        public DriverConfig DriverConfig { get; set; }
        public Labels Labels { get; set; }
    }

    public class Mount
    {
        public bool ReadOnly { get; set; }
        public string Source { get; set; }
        public string Target { get; set; }
        public string Type { get; set; }
        public VolumeOptions VolumeOptions { get; set; }
    }

    public class ContainerSpec
    {
        public string Image { get; set; }
        public List<Mount> Mounts { get; set; }
        public string User { get; set; }
    }

    public class Network
    {
        public string Target { get; set; }
    }

    public class Options
    {
        [JsonProperty("max-file")]
        public string MaxFile { get; set; }

        [JsonProperty("max-size")]
        public string MaxSize { get; set; }
    }

    public class LogDriver
    {
        public string Name { get; set; }
        public Options Options { get; set; }
    }

    public class Placement
    {
        public List<string> Constraints { get; set; }
    }

    public class Limits
    {
        public int MemoryBytes { get; set; }
    }

    public class Reservations
    {
    }

    public class Resources
    {
        public Limits Limits { get; set; }
        public Reservations Reservations { get; set; }
    }

    public class RestartPolicy
    {
        public string Condition { get; set; }
        public long Delay { get; set; }
        public int MaxAttempts { get; set; }
    }

    public class TaskTemplate
    {
        public ContainerSpec ContainerSpec { get; set; }
        public List<Network> Networks { get; set; }
        public LogDriver LogDriver { get; set; }
        public Placement Placement { get; set; }
        public Resources Resources { get; set; }
        public RestartPolicy RestartPolicy { get; set; }
    }

    public class Replicated
    {
        public int Replicas { get; set; }
    }

    public class Mode
    {
        public Replicated Replicated { get; set; }
    }

    public class UpdateConfig
    {
        public long Delay { get; set; }
        public int Parallelism { get; set; }
        public string FailureAction { get; set; }
    }

    public class Port
    {
        public string Protocol { get; set; }
        public int PublishedPort { get; set; }
        public int TargetPort { get; set; }
    }

    public class EndpointSpec
    {
        public List<Port> Ports { get; set; }
    }

    public class DockerService
    {
        public string Name { get; set; }
        public TaskTemplate TaskTemplate { get; set; }
        public Mode Mode { get; set; }
        public UpdateConfig UpdateConfig { get; set; }
        public EndpointSpec EndpointSpec { get; set; }
        public IDictionary<string, string> Labels { get; set; }
    }


}
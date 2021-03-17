using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core;
using Core.DTO;
using Newtonsoft.Json;
using System.Net.Http;
using System.IO;

namespace Docker
{
    public class DockerServiceHandle : IServiceHandle
    {
        private const string CREATE_URI = "/v1.24/services/create";
        private const string GET_URI = "/v1.24/services";
        private const string BASE_URI = "unix:///var/run/docker.sock";
        private const string MEDIA_TYPE = "application/json";
        private readonly HttpClient _httpClient;
        
        public DockerServiceHandle(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public void CreateService(MyService service)
        {
            var dockerService = service.GetDockerService();

            var json = JsonConvert.SerializeObject(dockerService);

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, CREATE_URI);

            requestMessage.Content = new StringContent(json, Encoding.UTF8, MEDIA_TYPE);

            var response = _httpClient.Send(requestMessage);

        }

        public IList<MyService> GetServices()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, GET_URI);

            var response = _httpClient.SendAsync(requestMessage).Result;

            var objs = JsonConvert.DeserializeObject<List<MyService>>(response.Content.ReadAsStringAsync().Result);
            
            System.Console.WriteLine();
            return default(List<MyService>);
        }

        public void RemoveService(string serviceName)
        {
            throw new System.NotImplementedException();
        }

        public void UpgradeService(string serviceName)
        {
            throw new System.NotImplementedException();
        }
    }
}
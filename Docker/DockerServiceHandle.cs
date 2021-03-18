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
using System.IO;
using System;

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

        public async void CreateService(MyService service)
        {
            var dockerService = service.GetDockerService();

            var json = JsonConvert.SerializeObject(dockerService);

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, CREATE_URI);

            requestMessage.Content = new StringContent(json, Encoding.UTF8, MEDIA_TYPE);

            var response = await _httpClient.SendAsync(requestMessage);

            if (response.StatusCode != HttpStatusCode.Created)
            {
                throw new InvalidOperationException(response.Content.ReadAsStringAsync().Result);
            }

        }

        public async Task<IEnumerable<MyService>> GetServices()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, GET_URI);

            var response = await _httpClient.SendAsync(requestMessage);

            var body = await response.Content.ReadAsStringAsync();

            var dockerServices = JsonConvert.DeserializeObject<IList<DockerServiceResponse>>(body);
            
            return dockerServices.GetMyServices();
        }

        public async void RemoveService(string serviceId)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, $"{GET_URI}/{serviceId}");

            var response = await _httpClient.SendAsync(requestMessage);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new InvalidOperationException(await response.Content.ReadAsStringAsync());
            }
        }

        public async void UpdateService(MyService service)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"{GET_URI}/{service.Id}/update");

            var serviceDocker = service.GetDockerService();

            var json = JsonConvert.SerializeObject(serviceDocker);

            requestMessage.Content = new StringContent(json, Encoding.UTF8, MEDIA_TYPE);           

            var response = await _httpClient.SendAsync(requestMessage);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new InvalidOperationException(await response.Content.ReadAsStringAsync());
            }
        }
    }
}
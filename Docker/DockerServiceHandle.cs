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
        private const string NETWORK_URI = "/v1.24/networks";
        private const string MEDIA_TYPE = "application/json";
        private readonly HttpClient _httpClient;
        
        public DockerServiceHandle(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> CreateService(MyService service, bool ensureNetworks)
        {
            var dockerService = service.GetDockerService().Spec.AsJson();

            if (ensureNetworks)
            {
                EnsureNetworks(service.Networks);
            }
            
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, CREATE_URI);

            requestMessage.Content = new StringContent(dockerService, Encoding.UTF8, MEDIA_TYPE);

            var response = await _httpClient.SendAsync(requestMessage);

            return await response.Content.ReadAsStringAsync();

        }

        private void EnsureNetworks(IEnumerable<string> networks)
        {
            
        }

        public async Task<IEnumerable<MyService>> GetServices()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, GET_URI);

            var response = await _httpClient.SendAsync(requestMessage);

            var body = await response.Content.ReadAsStringAsync();

            var dockerServices = body.FromJson<IList<DockerServiceResponse>>();
            
            return dockerServices.GetMyServices();
        }

        public async Task<string> RemoveService(string serviceId)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, $"{GET_URI}/{serviceId}");

            var response = await _httpClient.SendAsync(requestMessage);

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> UpdateService(MyService service)
        {
            var id = !string.IsNullOrEmpty(service.Id) ? service.Id : service.Name;

            var version = await GetLastServiceVersion(id);

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"{GET_URI}/{id}/update?version={version}");

            var serviceDocker = service.GetDockerService().Spec.AsJson();

            requestMessage.Content = new StringContent(serviceDocker, Encoding.UTF8, MEDIA_TYPE);           

            var response = await _httpClient.SendAsync(requestMessage);

            return await response.Content.ReadAsStringAsync();
        }

        private async Task<int> GetLastServiceVersion(string id)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"{GET_URI}/{id}");

            var response = await _httpClient.SendAsync(requestMessage);

            var body = await response.Content.ReadAsStringAsync();

            var service = body.FromJson<DockerServiceResponse>();

            return service.Version.Index;
        }
    }
}

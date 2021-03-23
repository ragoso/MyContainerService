using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Core;

namespace Docker
{
    public class DockerImageHandle : IImageHandle
    {
        private const string BUILD_URI = "/v1.24/build";
        private readonly HttpClient _httpClient;
        public DockerImageHandle(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<string> BuildImage(Stream tar, string tag)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"{BUILD_URI}?t={tag}");
            
            requestMessage.Content = new StreamContent(tar);

            var response = await _httpClient.SendAsync(requestMessage);

            return await response.Content.ReadAsStringAsync();

        }
    }
}
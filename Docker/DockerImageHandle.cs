using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
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
        public async Task<string> BuildImage(byte[] imageFile, string tag)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            query.Add("t", tag);
            query.Add("q", "true");
            //query.Add("dockerfile", "./Dockerfile");

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"{BUILD_URI}?{query.ToString()}");
            
            var tarAsString = Encoding.UTF8.GetString(imageFile);

            requestMessage.Content = new StringContent(tarAsString, Encoding.UTF8, "application/x-tar");

            var response = await _httpClient.SendAsync(requestMessage);

            return await response.Content.ReadAsStringAsync();

        }
    }
}
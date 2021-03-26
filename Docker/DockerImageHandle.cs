using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            _httpClient.Timeout = new TimeSpan(0, 15, 0);
        }
        public async Task<string> BuildImage(Stream stream, IEnumerable<string> param, string tag)
        {
            var buildArgs = GetBuildArgs(param);
            var query = HttpUtility.ParseQueryString(string.Empty);
            query.Add("t", tag);
            query.Add("q", "true");
            query.Add("buildargs", buildArgs);

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"{BUILD_URI}?{query.ToString()}");

            requestMessage.Content = new StreamContent(stream);

            var response = await _httpClient.SendAsync(requestMessage);

            return await response.Content.ReadAsStringAsync();
        }
        public async Task<string> BuildImage(byte[] imageFile, IEnumerable<string> param, string tag)
        {
            using var memStream = new MemoryStream(imageFile);

            return await BuildImage(memStream, param, tag);

        }

        private static string GetBuildArgs(IEnumerable<string> param)
        {
            var p = param.Select(x =>
            {
                var p = x.Split('=');
                return $"\"{p.FirstOrDefault()}\":\"{p.LastOrDefault()}\"";
            });

            var args = "{";
            args += $"{string.Join(',', p)}";
            args += "}";

            return args;
        }
    }
}
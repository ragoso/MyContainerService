using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core;
using Endpoint;

namespace Console
{
    internal class ImageClientHandle : ClientHandle, IImageHandle
    {
        private readonly MyContainerImage.MyContainerImageClient _client;
        private readonly int _bufferLength = 1 * 1024;
        private readonly string _token;
        public ImageClientHandle(MyContainerImage.MyContainerImageClient client, string token): base(token)
        {
            _client = client;
            _token = token;
        }

        public ImageClientHandle(MyContainerImage.MyContainerImageClient client, string token, int bufferLength): this(client, token)
        {
            _bufferLength = bufferLength;
        }

        public async Task<string> BuildImage(byte[] imageFile, IEnumerable<string> param, string tag)
        {
            var request = CreateRequest(imageFile, param, tag);

            var reply = await Task.Run(() => _client.Build(request));

            return reply.Message;
        }

        public async Task<string> BuildImage(Stream stream, IEnumerable<string> param, string tag)
        {
            var client = _client.BuildStream();

            byte[] buffer = new byte[_bufferLength];

            int bytesRead = 0;

            while((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                var request = CreateRequest(buffer, param, tag);

                await client.RequestStream.WriteAsync(request);

            }

            await client.RequestStream.CompleteAsync();

            return client.ResponseAsync.Result.Message;
        }

        private static BuildRequest CreateRequest(byte[] imageFile, IEnumerable<string> param, string tag)
        {
            var request = new BuildRequest()
            {
                Tag = tag,
                TarFile = Google.Protobuf.ByteString.CopyFrom(imageFile)
            };

            if (param?.Any() ?? false)
            {
                request.Params.AddRange(param);
            }

            return request;
        }
    }
}
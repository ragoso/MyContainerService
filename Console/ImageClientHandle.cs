using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Core;
using Core.DTO;
using Endpoint;
using Grpc.Core;
using GRPC;

namespace Console
{
    internal class ImageClientHandle : ClientHandle, IImageHandle
    {
        private readonly MyContainerImage.MyContainerImageClient _client;
        private readonly string _token;
        public ImageClientHandle(MyContainerImage.MyContainerImageClient client, string token): base(token)
        {
            _client = client;
            _token = token;
        }

        public async Task<string> BuildImage(Stream tar, string tag)
        {
            using var stream = _client.Build();

            var fileByte = Google.Protobuf.ByteString.FromStream(tar);

            await Task.Run(() => stream.RequestStream.WriteAsync(new BuildRequest()
            {
                Tag = tag,
                TarFile = fileByte
            }));

            return stream.ResponseAsync.Result.Message;
        }
    }
}
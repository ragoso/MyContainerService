using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public async Task<string> BuildImage(byte[] imageFile, IEnumerable<string> param, string tag)
        {
            var fileByte = Google.Protobuf.ByteString.CopyFrom(imageFile);

            var request = new BuildRequest()
            {
                Tag = tag,
                TarFile = fileByte
            };

            if (param?.Any() ?? false)
            {
                request.Params.AddRange(param);
            }

            var reply = await Task.Run(() => _client.Build(request));



            return reply.Message;
        }
    }
}
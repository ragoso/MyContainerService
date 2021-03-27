using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core;
using Google.Protobuf;
using Grpc.Core;
using GRPC;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Endpoint
{
    //[Authorize(AuthenticationSchemes = 
    //JwtBearerDefaults.AuthenticationScheme)]
    public  class MyContainerImageImpl : MyContainerImage.MyContainerImageBase
    {
        private readonly IImageHandle _handle;
        private readonly ILogger<MyContainerImageImpl> _logger;

        public MyContainerImageImpl(ILogger<MyContainerImageImpl> logger, IImageHandle handle)
        {
            _logger = logger;
            _handle = handle;
        }

        public override async Task<BuildReply> Build(BuildRequest request, ServerCallContext context)
        {
            var data = request.TarFile.ToByteArray();
            
            var tag = request.Tag;
            var param = request.Params.Select(x => x);
            var response = await _handle.BuildImage(data, param, tag);

            return new BuildReply()
            {
                Message = response
            };
        }

        public override async Task<BuildReply> BuildStream(IAsyncStreamReader<BuildRequest> requestStream, ServerCallContext context)
        {
            using var stream = new MemoryStream();
               
            var param = requestStream.Current.Params;
            var tag = requestStream.Current.Tag;

            while(await requestStream.MoveNext())
            {
                requestStream.Current.WriteTo(stream);
            }
            
            var response = await _handle.BuildImage(stream, param, tag);
            
            return new BuildReply()
            {
                Message = response
            };
           
        }

    }
}

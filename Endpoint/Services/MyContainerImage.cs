using System.IO;
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

        public override async Task<BuildReply> Build(IAsyncStreamReader<BuildRequest> requestStream, ServerCallContext context)
        {
            using(var memStream = new MemoryStream())
            {
                var tarFile = new CodedOutputStream(memStream);
                requestStream.Current.WriteTo(tarFile);
                
                var tag = requestStream.Current.Tag;
                var response = await _handle.BuildImage(memStream, tag);

                return new BuildReply()
                {
                    Message = response
                };
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Endpoint
{
    public partial class MyContainerServiceImpl : MyContainerService.MyContainerServiceBase
    {
        private readonly IServiceHandle _handle;
        private readonly ILogger<MyContainerServiceImpl> _logger;
        public MyContainerServiceImpl(ILogger<MyContainerServiceImpl> logger, IServiceHandle handle)
        {
            _logger = logger;
            _handle = handle;
        }

        public override async Task<CreateReply> Create(CreateRequest request, ServerCallContext context)
        {
            var msg = await _handle.CreateService(request.Service.FromGrpcService());
            return new CreateReply()
            {
                Message = msg
            };
        }

        public override async Task<GetReply> Get(GetRequest request, ServerCallContext context)
        {
            var reply = new GetReply();

            reply.Services.AddRange((await _handle.GetServices()).ToGrpcService());

            return reply;
        }

        public override async Task<RemoveReply> Remove(RemoveRequest request, ServerCallContext context)
        {
            return new RemoveReply()
            {
                Message = await _handle.RemoveService(request.ServiceNameOrId)
            };
        }

        public override async Task<UpdateReply> Update(UpdateRequest request, ServerCallContext context)
        {
            return new UpdateReply()
            {
                Message = await _handle.UpdateService(request.Service.FromGrpcService())
            };
        }
    }
}

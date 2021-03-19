using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Endpoint
{
    public partial class MyContainerServiceImpl : MyContainerService.MyContainerServiceBase
    {
        private readonly ILogger<MyContainerServiceImpl> _logger;
        public MyContainerServiceImpl(ILogger<MyContainerServiceImpl> logger)
        {
            _logger = logger;
        }

        public override Task<UpdateReply> Update(UpdateRequest request, ServerCallContext context)
        {
            return Task.FromResult(new UpdateReply
            {
                Message = "Hello " + request.Service.Name
            });
        }
    }
}

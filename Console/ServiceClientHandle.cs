using System.Collections.Generic;
using System.Threading.Tasks;
using Core;
using Core.DTO;
using Endpoint;
using Grpc.Core;
using GRPC;

namespace Console
{
    internal class ServiceClientHandle : ClientHandle, IServiceHandle
    {
        private readonly MyContainerService.MyContainerServiceClient _client;
        
        public ServiceClientHandle(MyContainerService.MyContainerServiceClient client, string token): base(token)
        {
            _client = client;
        }
        public async Task<string> CreateService(MyService service, bool ensureNetworks)
        {
            var reply = await Task.Run(() =>  _client.Create(new CreateRequest()
            {
                Service = service.ToGrpcService()
            }, GetTokenHeader()));

            return reply.Message;
        }

        public Task<IEnumerable<MyService>> GetServices()
        {
            throw new System.NotImplementedException();
        }

        public async Task<string> RemoveService(string serviceId)
        {
             var reply = await Task.Run (() => _client.Remove(new RemoveRequest() {
                ServiceNameOrId = serviceId
            }, GetTokenHeader()));

            return reply.Message;
        }

        public async Task<string> UpdateService(MyService service)
        {
            var reply = await Task.Run(() => _client.Update(new UpdateRequest()
            {
                Service = service.ToGrpcService()
            }, GetTokenHeader()));

            return reply.Message;
        }
    }
}
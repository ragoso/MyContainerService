
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.DTO;

namespace Core
{
    public interface IServiceHandle
    {
        Task<string> CreateService(MyService service);
        Task<string> UpdateService(MyService service);
        Task<string> RemoveService(string serviceId);
        Task<IEnumerable<MyService>> GetServices();
    }
    
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Core.DTO;

namespace Core
{
    public interface IServiceHandle
    {
        void CreateService(MyService service);
        void UpdateService(MyService service);
        void RemoveService(string serviceId);
        Task<IEnumerable<MyService>> GetServices();
    }
    
}
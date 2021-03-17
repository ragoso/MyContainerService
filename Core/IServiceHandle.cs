
using System.Collections.Generic;
using Core.DTO;

namespace Core
{
    public interface IServiceHandle
    {
        void CreateService(MyService service);
        void UpgradeService(string serviceName);
        void RemoveService(string serviceName);
        IList<MyService> GetServices();
    }
    
}
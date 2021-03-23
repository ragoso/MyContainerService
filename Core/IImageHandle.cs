using System.IO;
using System.Threading.Tasks;

namespace Core
{
    public interface IImageHandle
    {
        Task<string> BuildImage(Stream tar, string tag);
    }
}
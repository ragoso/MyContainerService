using System.IO;
using System.Threading.Tasks;

namespace Core
{
    public interface IImageHandle
    {
        Task<string> BuildImage(byte[] imageFile, string tag);
    }
}
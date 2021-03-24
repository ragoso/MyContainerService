using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Core
{
    public interface IImageHandle
    {
        Task<string> BuildImage(byte[] imageFile, IEnumerable<string> param, string tag);
    }
}
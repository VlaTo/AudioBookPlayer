using System.IO;
using System.Threading.Tasks;

namespace AudioBookPlayer.App.Core.Services
{
    public interface ISourceStreamProvider
    {
        Task<Stream> GetStreamAsync(); 
    }
}

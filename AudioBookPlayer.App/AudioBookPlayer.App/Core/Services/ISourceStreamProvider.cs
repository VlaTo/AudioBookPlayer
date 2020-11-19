using System.IO;
using System.Threading.Tasks;

namespace AudioBookPlayer.App.Core.Services
{
    public interface ISourceStreamProvider
    {
        string GetBookPath();

        Task<Stream> GetStreamAsync(); 
    }
}

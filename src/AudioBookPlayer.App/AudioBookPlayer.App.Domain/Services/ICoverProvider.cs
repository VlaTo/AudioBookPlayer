using System.IO;
using System.Threading.Tasks;

namespace AudioBookPlayer.App.Domain.Services
{
    public interface ICoverProvider
    {
        Task<string> AddCoverAsync(long bookId, Stream stream);
    }
}
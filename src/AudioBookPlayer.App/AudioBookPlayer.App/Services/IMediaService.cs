using System.Threading.Tasks;

namespace AudioBookPlayer.App.Services
{
    public interface IMediaService
    {
        Task LoadMediaAsync();

        Task<string> GetRootFolderAsync();
    }
}

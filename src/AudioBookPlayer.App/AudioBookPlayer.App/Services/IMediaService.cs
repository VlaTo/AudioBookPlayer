using System.Threading.Tasks;

namespace AudioBookPlayer.App.Services
{
    public interface IMediaService
    {
        void LoadMedia();

        Task<string> GetRootFolderAsync();
    }
}

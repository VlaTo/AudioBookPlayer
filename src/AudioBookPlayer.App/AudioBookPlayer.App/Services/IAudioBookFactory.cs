using AudioBookPlayer.App.Models;

namespace AudioBookPlayer.App.Services
{
    public interface IAudioBookFactory
    {
        AudioBook CreateAudioBook(string folder, string filename, int level);
    }
}
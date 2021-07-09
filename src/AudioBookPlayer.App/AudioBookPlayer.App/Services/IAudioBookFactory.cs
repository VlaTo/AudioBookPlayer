using System.IO;
using LibraProgramming.Media.Common;

namespace AudioBookPlayer.App.Services
{
    public interface IAudioBookFactory
    {
        MediaInfo ExtractMediaInfo(Stream stream);
    }
}
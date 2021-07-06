using System.IO;
using LibraProgramming.Media.Common;

namespace AudioBookPlayer.App.Services
{
    public interface IAudioBookFactory
    {
        MediaInformation ExtractMediaInfo(Stream stream);
    }
}
using System.IO;
using LibraProgramming.Media.Common;

namespace AudioBookPlayer.App.Services
{
    public interface IMediaInfoProvider
    {
        MediaInfo ExtractMediaInfo(Stream stream);
    }
}
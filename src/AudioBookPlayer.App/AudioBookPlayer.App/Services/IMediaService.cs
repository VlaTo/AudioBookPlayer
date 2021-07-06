using System.Collections.Generic;
using System.Threading.Tasks;
using AudioBookPlayer.App.Models;

namespace AudioBookPlayer.App.Services
{
    public interface IMediaService
    {
        Task<IEnumerable<AudioBook>> QueryBooksAsync();
    }
}

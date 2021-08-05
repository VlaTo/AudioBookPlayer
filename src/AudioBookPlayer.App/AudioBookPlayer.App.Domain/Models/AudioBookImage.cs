using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AudioBookPlayer.App.Domain.Data;

namespace AudioBookPlayer.App.Domain.Models
{
    public abstract class AudioBookImage : IEntity
    {
        public AudioBook AudioBook
        {
            get;
        }

        protected AudioBookImage(AudioBook audioBook)
        {
            AudioBook = audioBook;
        }

        public abstract Task<Stream> GetStreamSync(CancellationToken cancellationToken = default);
    }
}
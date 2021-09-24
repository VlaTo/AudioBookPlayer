using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AudioBookPlayer.App.Domain.Core;
using AudioBookPlayer.App.Domain.Providers;

namespace AudioBookPlayer.App.Domain.Models
{
    public sealed class ContentProvidedAudioBookImage : AudioBookImage, IHasContentUri
    {
        private readonly ICoverProvider provider;

        public string ContentUri
        {
            get;
        }

        public ContentProvidedAudioBookImage(AudioBook audioBook, string contentUri, ICoverProvider provider)
            : base(audioBook)
        {
            this.provider = provider;
            ContentUri = contentUri;
        }

        public override Task<Stream> GetStreamAsync(CancellationToken cancellationToken = default)
        {
            var stream = provider.GetImageAsync(ContentUri, cancellationToken);
            return stream;
        }

        public override Stream GetStream()
        {
            var stream = provider.GetImage(ContentUri);
            return stream;
        }
    }
}
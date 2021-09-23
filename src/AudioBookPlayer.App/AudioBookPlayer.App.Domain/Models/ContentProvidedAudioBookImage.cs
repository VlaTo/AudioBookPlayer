using AudioBookPlayer.App.Domain.Services;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AudioBookPlayer.App.Domain.Core;

namespace AudioBookPlayer.App.Domain.Models
{
    public sealed class ContentProvidedAudioBookImage : AudioBookImage, IHasContentUri
    {
        private readonly ICoverService provider;

        public string ContentUri
        {
            get;
        }

        public ContentProvidedAudioBookImage(AudioBook audioBook, string contentUri, ICoverService provider)
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
    }
}
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AudioBookPlayer.App.Domain.Services;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace AudioBookPlayer.App.Services
{
    internal sealed class AudioBooksManager
    {
        private readonly IMediaLibrary mediaLibrary;
        private readonly ICoverProvider coverProvider;

        public AudioBooksManager(IMediaLibrary mediaLibrary, ICoverProvider coverProvider)
        {
            this.mediaLibrary = mediaLibrary;
            this.coverProvider = coverProvider;
        }

        public async Task ApplyChangesAsync(IReadOnlyList<LibraryChange> changes, CancellationToken cancellationToken = default)
        {
            for (var changeIndex = 0; changeIndex < changes.Count; changeIndex++)
            {
                var change = changes[changeIndex];

                switch (change.Action)
                {
                    case ChangeAction.Add:
                    {
                        await mediaLibrary.SaveBookAsync(change.Source, cancellationToken);

                        var bookId = change.Source.Id.Value;
                        var images = change.Source.Images;

                        for (var imageIndex = 0; imageIndex < images.Count; imageIndex++)
                        {
                            var image = images[imageIndex];
                            var stream = await image.GetStreamSync(cancellationToken);
                            var coverUri = await coverProvider.AddCoverAsync(bookId, stream);

                        }

                        break;
                    }

                    default:
                    {
                        break;
                    }
                }
            }

            await Task.CompletedTask;
        }
    }
}
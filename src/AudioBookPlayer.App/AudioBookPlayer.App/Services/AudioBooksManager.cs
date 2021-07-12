using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AudioBookPlayer.App.Services
{
    internal sealed class AudioBooksManager
    {
        private readonly IMediaLibrary mediaLibrary;

        public AudioBooksManager(IMediaLibrary mediaLibrary)
        {
            this.mediaLibrary = mediaLibrary;
        }

        public async Task ApplyChangesAsync(IReadOnlyList<LibraryChange> changes, CancellationToken cancellationToken = default)
        {
            for (var index = 0; index < changes.Count; index++)
            {
                var change = changes[index];

            }

            await Task.CompletedTask;
        }
    }
}
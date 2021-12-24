namespace AudioBookPlayer.MediaBrowserService.Core
{
    internal sealed class MediaInfoProviderFactory
    {
        public MediaInfoProvider CreateProviderFor(string extension, string mimeType)
        {
            var ext = extension.ToLowerInvariant();

            switch (ext)
            {
                case ".m4b":
                {
                    return new QuickTimeMediaInfoProvider(mimeType);
                }
            }

            return null;
        }
    }
}
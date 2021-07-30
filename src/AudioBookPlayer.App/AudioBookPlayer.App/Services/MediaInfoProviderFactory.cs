namespace AudioBookPlayer.App.Services
{
    internal sealed class MediaInfoProviderFactory : IMediaInfoProviderFactory
    {
        public MediaInfoProviderFactory()
        {
        }

        public IMediaInfoProvider CreateProviderFor(string extension)
        {
            var ext = extension.ToLowerInvariant();

            switch (ext)
            {
                case ".m4b":
                {
                    return new QuickTimeMediaInfoProvider();
                }
            }

            return null;
        }
    }
}
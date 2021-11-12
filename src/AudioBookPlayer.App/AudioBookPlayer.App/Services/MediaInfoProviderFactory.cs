﻿namespace AudioBookPlayer.App.Services
{
    public sealed class MediaInfoProviderFactory : IMediaInfoProviderFactory
    {
        public IMediaInfoProvider CreateProviderFor(string extension, string mimeType)
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
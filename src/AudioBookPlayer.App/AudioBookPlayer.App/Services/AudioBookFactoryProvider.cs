namespace AudioBookPlayer.App.Services
{
    internal sealed class AudioBookFactoryProvider : IAudioBookFactoryProvider
    {
        public AudioBookFactoryProvider()
        {
        }

        public IAudioBookFactory CreateFactoryFor(string extension)
        {
            var ext = extension.ToLowerInvariant();

            switch (ext)
            {
                case ".m4b":
                {
                    return new QuickTimeAudioBookFactory();
                }
            }

            return null;
        }
    }
}
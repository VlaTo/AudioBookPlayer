using System;

namespace AudioBookPlayer.App.Android.Services
{
    public sealed partial class PlaybackService
    {
        private sealed class UpdateToken : IDisposable
        {
            private readonly PlaybackService service;
            private bool disposed;

            public UpdateToken(PlaybackService service)
            {
                this.service = service;
            }

            public void Dispose()
            {
                if (disposed)
                {
                    return;
                }

                try
                {
                    service.EndUpdate();
                }
                finally
                {
                    disposed = true;
                }
            }
        }
    }
}
namespace AudioBookPlayer.App.Android.Services
{
    public sealed partial class AudioBooksPlaybackService
    {
        public interface ICustomPlayback
        {
            public const int PositionChangedEvent = PlaybackPositionChangedEvent;
            public const int QueueIndexChangedEvent = PlaybackQueueIndexChangedEvent;
            public const string PositionKey = ParamsPositionKey;
            public const string DurationKey = ParamsDurationKey;
            public const string QueueIndexKey = ParamsQueueIndexKey;

            void OnPlaybackPositionChanged(double position, double duration);

            void OnQueueIndexChanged(int queueIndex);
        }
    }
}
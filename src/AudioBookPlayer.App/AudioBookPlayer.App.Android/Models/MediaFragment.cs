namespace AudioBookPlayer.App.Android.Models
{
    internal readonly struct MediaFragment
    {
        public long Start
        {
            get;
        }

        public long Duration
        {
            get;
        }

        public MediaFragment(long start, long duration)
        {
            Start = start;
            Duration = duration;
        }

        public void Deconstruct(out long start, out long duration)
        {
            start = Start;
            duration = Duration;
        }
    }
}
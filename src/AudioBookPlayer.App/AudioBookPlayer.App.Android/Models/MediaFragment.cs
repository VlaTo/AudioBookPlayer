namespace AudioBookPlayer.App.Android.Models
{
    internal readonly struct MediaFragment
    {
        public double Start
        {
            get;
        }

        public double Duration
        {
            get;
        }

        public MediaFragment(double start, double duration)
        {
            Start = start;
            Duration = duration;
        }

        public void Deconstruct(out double start, out double duration)
        {
            start = Start;
            duration = Duration;
        }
    }
}
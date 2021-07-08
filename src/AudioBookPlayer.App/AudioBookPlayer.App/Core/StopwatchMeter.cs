using System;
using System.Diagnostics;

namespace AudioBookPlayer.App.Core
{
    public sealed class StopwatchMeter : IDisposable
    {
        private readonly Stopwatch stopwatch;
        private readonly string message;
        private bool stopped;

        private StopwatchMeter(Stopwatch stopwatch, string message)
        {
            this.stopwatch = stopwatch;
            this.message = message;
        }

        public static StopwatchMeter Start(string message)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            return new StopwatchMeter(stopwatch, message);
        }

        public void Dispose()
        {
            if (!stopped)
            {
                stopwatch.Stop();
                stopped = true;
            }

            Debug.WriteLine($"[StopwatchMeter] Execution of '{message}' took: '{stopwatch.Elapsed:g}'");
        }
    }
}
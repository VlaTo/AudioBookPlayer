using System;
using System.Collections.Generic;

namespace AudioBookPlayer.MediaBrowserService.Core
{
    internal readonly struct ProgressStep
    {
        public int Key
        {
            get;
        }

        public float Weight
        {
            get;
        }

        public ProgressStep(int key, float weight)
        {
            Key = key;
            Weight = weight;
        }
    }

    internal sealed class MultiStepProgress
    {
        private readonly Dictionary<int, Reporter> reporters;
        private readonly List<(Subscription, IProgressEx<int, float>)> handlers;

        public IProgress<float> this[int key]
        {
            get
            {
                if (reporters.TryGetValue(key, out var reporter))
                {
                    return reporter;
                }

                throw new KeyNotFoundException();
            }
        }

        public MultiStepProgress(ProgressStep[] steps)
        {
            reporters = new Dictionary<int, Reporter>();
            handlers = new List<(Subscription, IProgressEx<int, float>)>();

            for (var index = 0; index < steps.Length; index++)
            {
                reporters[steps[index].Key] = new Reporter(this, steps[index].Key, steps[index].Weight);
            }
        }

        public IDisposable Subscribe(IProgressEx<int, float> progress)
        {
            var disposable = new Subscription(this);
            
            handlers.Add((disposable, progress));
            
            return disposable;
        }
        
        public IDisposable Subscribe(Action<int, float> callback)
        {
            var reporter = new CallbackReporter(callback);
            return Subscribe(reporter);
        }

        private void UpdateProgress(int key)
        {
            var progress = GetOverallProgress();
            RaiseCallbacks(key, progress);
        }

        private void RaiseCallbacks(int key, float progress)
        {
            var callbacks = handlers.ToArray();

            for (var index = 0; index < callbacks.Length; index++)
            {
                var (_, callback) = callbacks[index];
                callback.Report(key, progress);
            }
        }

        private float GetOverallProgress()
        {
            var progress = 0.0f;

            foreach (var kvp in reporters)
            {
                var reporter = kvp.Value;
                progress += (reporter.Weight * reporter.Progress);
            }

            return progress;
        }

        private void RemoveSubscription(Subscription subscription)
        {
            for (var index = 0; index < handlers.Count; index++)
            {
                var (disposable, _) = handlers[index];

                if (ReferenceEquals(subscription, disposable))
                {
                    handlers.RemoveAt(index);
                    return;
                }
            }
        }

        private sealed class Reporter : IProgress<float>
        {
            private readonly MultiStepProgress owner;

            public int Key
            {
                get;
            }

            public float Weight
            {
                get;
            }

            public float Progress
            {
                get;
                set;
            }

            public Reporter(MultiStepProgress owner, int key, float weight)
            {
                this.owner = owner;
                Key = key;
                Weight = weight;
            }

            public void Report(float value)
            {
                Progress = value;
                owner.UpdateProgress(Key);
            }
        }

        private sealed class CallbackReporter : IProgressEx<int, float>
        {
            private readonly Action<int, float> callback;

            public CallbackReporter(Action<int, float> callback)
            {
                this.callback = callback;
            }

            public void Report(int key, float progress) => callback.Invoke(key, progress);
        }

        private sealed class Subscription : IDisposable
        {
            private MultiStepProgress owner;
            private bool disposed;

            public Subscription(MultiStepProgress owner)
            {
                this.owner = owner;
            }

            public void Dispose()
            {
                Disposable(true);
            }

            private void Disposable(bool dispose)
            {
                if (disposed)
                {
                    return;
                }

                try
                {
                    if (dispose)
                    {
                        owner.RemoveSubscription(this);
                        owner = null;
                    }
                }
                finally
                {
                    disposed = true;
                }
            }
        }
    }
}
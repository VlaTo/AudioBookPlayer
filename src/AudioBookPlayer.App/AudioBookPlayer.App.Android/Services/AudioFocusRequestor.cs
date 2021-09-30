using System;
using Android.Media;

namespace AudioBookPlayer.App.Android.Services
{
    public enum AudioFocusState : short
    {
        None,
        Acquired,
        CanDuck
    }

    internal sealed class AudioFocusRequestor : Java.Lang.Object, AudioManager.IOnAudioFocusChangeListener
    {
        private readonly AudioManager audioManager;
        private readonly AudioAttributes audioAttributes;
        private readonly Action<bool> callback;
        private AudioFocusRequestClass audioFocusRequest;

        public AudioFocusState State
        {
            get;
            private set;
        }

        public AudioFocusRequestor(AudioManager audioManager, AudioAttributes audioAttributes, Action<bool> callback)
        {
            this.audioManager = audioManager;
            this.audioAttributes = audioAttributes;
            this.callback = callback;

            State = AudioFocusState.None;
        }

        public AudioFocusRequest Acquire()
        {
            if (null == audioFocusRequest)
            {
                audioFocusRequest = new AudioFocusRequestClass.Builder(AudioFocus.Gain)
                    .SetAudioAttributes(audioAttributes)
                    .SetOnAudioFocusChangeListener(this)
                    .SetWillPauseWhenDucked(false)
                    .Build();
            }

            if (null != audioFocusRequest)
            {
                var result = audioManager.RequestAudioFocus(audioFocusRequest);

                if (AudioFocusRequest.Granted == result)
                {
                    State = AudioFocusState.Acquired;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("[AndroidPlaybackService] [Acquire] Audio focus not acquired!");
                }

                return result;
            }

            return AudioFocusRequest.Failed;
        }

        public void Release()
        {
            if (null == audioFocusRequest)
            {
                return;
            }

            var result = audioManager.AbandonAudioFocusRequest(audioFocusRequest);

            if (AudioFocusRequest.Granted == result)
            {
                State = AudioFocusState.None;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("[AndroidPlaybackService] [Release] Cannot release Audio focus.");
            }

            audioFocusRequest = null;
        }

        void AudioManager.IOnAudioFocusChangeListener.OnAudioFocusChange(AudioFocus focusChange)
        {
            var canDuck = false;

            if (AudioFocus.Gain == focusChange)
            {
                State = AudioFocusState.Acquired;
            }
            else
            if (AudioFocus.Loss == focusChange || AudioFocus.LossTransient == focusChange || AudioFocus.LossTransientCanDuck == focusChange)
            {
                canDuck = AudioFocus.LossTransientCanDuck == focusChange;
                State = canDuck ? AudioFocusState.CanDuck : AudioFocusState.None;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[AndroidPlaybackService] [OnAudioFocusChange] Ignoring unsupported focusChange: {focusChange}");
            }

            callback.Invoke(canDuck);
        }
    }
}
using System;
using Android.Media;
using Android.Runtime;

namespace AudioBookPlayer.App.Android.Services
{
    internal sealed class AudioFocusRequestor : Java.Lang.Object, AudioManager.IOnAudioFocusChangeListener
    {
        private readonly AudioManager audioManager;
        private readonly AudioAttributes audioAttributes;
        private readonly Action<AudioFocus> callback;
        private AudioFocusRequestClass audioFocusRequest;

        public AudioFocusRequestor(AudioManager audioManager, AudioAttributes audioAttributes, Action<AudioFocus> callback)
        {
            this.audioManager = audioManager;
            this.audioAttributes = audioAttributes;
            this.callback = callback;
        }

        public AudioFocusRequest Acquire()
        {
            if (null == audioFocusRequest)
            {
                audioFocusRequest = new AudioFocusRequestClass.Builder(AudioFocus.GainTransientMayDuck)
                    .SetAudioAttributes(audioAttributes)
                    .Build();
            }

            var result = audioManager.RequestAudioFocus(audioFocusRequest);

            if (AudioFocusRequest.Granted != result)
            {
                System.Diagnostics.Debug.WriteLine($"[AndroidPlaybackService] [StartPlayFile] Audio focus not acquired!");
            }

            return result;
        }

        public void Release()
        {
            if (null == audioFocusRequest)
            {
                return;
            }

            var result = audioManager.AbandonAudioFocusRequest(audioFocusRequest);

            if (AudioFocusRequest.Granted != result)
            {
                System.Diagnostics.Debug.WriteLine($"[AndroidPlaybackService] [StartPlayFile] Audio focus not acquired!");
            }

            audioFocusRequest = null;
        }

        void AudioManager.IOnAudioFocusChangeListener.OnAudioFocusChange([GeneratedEnum] AudioFocus focusChange)
        {
            System.Diagnostics.Debug.WriteLine($"[AndroidPlaybackService] [OnAudioFocusChange] change: {focusChange}");
            callback.Invoke(focusChange);
        }
    }
}
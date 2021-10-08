#nullable enable

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Media;
using Android.OS;
using Android.Support.V4.Media.Session;
using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Domain.Services;
using System;
using AudioBookPlayer.App.Domain.Models;
using Application = Android.App.Application;
using Uri = Android.Net.Uri;

namespace AudioBookPlayer.App.Android.Services
{
    internal interface IPlaybackCallback
    {
        void StateChanged();
    }

    internal sealed partial class Playback : Java.Lang.Object, MediaPlayer.IOnPreparedListener,
        MediaPlayer.IOnCompletionListener, MediaPlayer.IOnErrorListener, MediaPlayer.IOnSeekCompleteListener
    {
        private const float VolumeNormal = 1.0f;
        private const float VolumeDuck = 0.6f;
        private const string ModeRead = "r";

        private readonly Service service;
        private readonly MediaSessionCompat mediaSession;
        private readonly IPlaybackCallback callback;
        private readonly AudioFocusRequestor audioFocusRequestor;
        private readonly BroadcastReceiver noisyReceiver;
        private readonly IntentFilter audioNoisyIntentFilter;
        private readonly AudioAttributes? audioAttributes;
        private Intent? noisyIntent;
        private MediaPlayer? mediaPlayer;
        private int state;
        // private long currentPosition;
        private bool playOnFocusGain;

        public Uri? MediaUri
        {
            get;
            private set;
        }

        public long FragmentStart
        {
            get;
            private set;
        }

        public long FragmentDuration
        {
            get;
            private set;
        }

        public long FragmentPosition
        {
            get;
            private set;
        }

        public int State
        {
            get => state;
            set
            {
                if (state == value)
                {
                    return;
                }

                state = value;
                callback.StateChanged();
            }
        }

        public bool IsConnected => true;

        public bool IsPlaying => playOnFocusGain || mediaPlayer is { IsPlaying: true };

        public long CurrentMediaPosition => null != mediaPlayer ? mediaPlayer.CurrentPosition : FragmentPosition;

        public Playback(Service service, MediaSessionCompat mediaSession, IPlaybackCallback callback)
        {
            this.service = service;
            this.mediaSession = mediaSession;
            this.callback = callback;

            state = PlaybackStateCompat.StateNone;
            noisyIntent = null;
            audioAttributes = CreateAudioAttributes();

            var audioManager = GetAudioManager();

            audioNoisyIntentFilter = new IntentFilter(AudioManager.ActionAudioBecomingNoisy);
            noisyReceiver = new BroadcastReceiver
            {
                OnReceiveImpl = (context, intent) =>
                {
                    if (String.Equals(AudioManager.ActionAudioBecomingNoisy, intent?.Action))
                    {
                        if (IsPlaying)
                        {
                            var action = new Intent(context, Java.Lang.Class.FromType(typeof(AudioBooksPlaybackService)));
                            
                            action.SetAction(AudioBooksPlaybackService.IActions.Command);
                            action.PutExtra(AudioBooksPlaybackService.IKeys.CommandName, AudioBooksPlaybackService.ICommands.Pause);
                            
                            service.StartService(action);
                        }
                    }
                }
            };
            audioFocusRequestor = new AudioFocusRequestor(audioManager, audioAttributes, DoAudioFocusChange);

            MediaUri = null;
            FragmentStart = -1L;
            FragmentDuration = 0L;
            FragmentPosition = FragmentStart;
        }

        public void PlayFragment(Uri mediaUri, double start, double duration)
        {
            if (AudioFocusRequest.Granted != audioFocusRequestor.Acquire())
            {
                return;
            }

            RegisterNoisyReceiver();

            if (false == mediaSession.Active)
            {
                mediaSession.Active = true;
            }

            var mediaHasChanged = MediaUri != mediaUri;

            if (mediaHasChanged)
            {
                Stop();

            }

            playOnFocusGain = true;

            if (mediaHasChanged && null != mediaPlayer)
            {
                UpdateMediaPlayerState();
            }

            MediaUri = mediaUri;
            FragmentStart = (long)start;
            FragmentDuration = (long)duration;
            FragmentPosition = FragmentStart;

            try
            {
                if (null != MediaUri)
                {
                    EnsureMediaPlayerCreated();

                    if (null != mediaPlayer)
                    {
                        State = PlaybackStateCompat.StateBuffering;

                        mediaPlayer.SetAudioAttributes(audioAttributes);
                        mediaPlayer.SetDataSource(Application.Context, MediaUri);
                        mediaPlayer.PrepareAsync();
                    }
                    else
                    {
                        State = PlaybackStateCompat.StateError;
                    }
                }
                else
                {
                    State = PlaybackStateCompat.StateError;
                }
            }
            catch
            {
                State = PlaybackStateCompat.StateError;
                throw;
            }
        }

        public void Play()
        {
            if (PlaybackStateCompat.StatePaused == State)
            {
                if (mediaPlayer is { IsPlaying: false })
                {
                    if (AudioFocusRequest.Granted != audioFocusRequestor.Acquire())
                    {
                        return;
                    }

                    RegisterNoisyReceiver();

                    mediaPlayer.Start();
                    FragmentPosition = mediaPlayer.CurrentPosition;
                }

                State = PlaybackStateCompat.StatePlaying;

                RegisterNoisyReceiver();
            }
        }

        public void Pause()
        {
            if (PlaybackStateCompat.StatePlaying == State)
            {
                if (mediaPlayer is { IsPlaying: true })
                {
                    mediaPlayer.Pause();
                    FragmentPosition = mediaPlayer.CurrentPosition;
                }

                // RelaxResources(false);
                audioFocusRequestor.Release();
            }

            State = PlaybackStateCompat.StatePaused;

            UnregisterNoisyReceiver();
        }

        public void Stop()
        {
            if (PlaybackStateCompat.StatePlaying == State)
            {
                if (mediaPlayer is { IsPlaying: true })
                {
                    mediaPlayer.Stop();
                    FragmentPosition = mediaPlayer.CurrentPosition;
                }

                // RelaxResources(false);
                audioFocusRequestor.Release();
            }

            State = PlaybackStateCompat.StateStopped;

            UnregisterNoisyReceiver();
        }

        /*private MediaFragmentInfo? GetPositionFromQueueItem()
        {
            if (null == QueueItem)
            {
                return null;
            }

            var start = (long)QueueItem.Description.Extras.GetDouble("Start");
            var duration = (long)QueueItem.Description.Extras.GetDouble("Duration");
            var mediaUri = QueueItem.Description.MediaUri;

            return new MediaFragmentInfo(start, duration, mediaUri);
        }*/

        private void DoAudioFocusChange(bool canDuck)
        {
            playOnFocusGain |= PlaybackStateCompat.StatePlaying == State && !canDuck;
            UpdateMediaPlayerState();
        }

        void MediaPlayer.IOnPreparedListener.OnPrepared(MediaPlayer? _)
        {
            UpdateMediaPlayerState();
        }

        void MediaPlayer.IOnCompletionListener.OnCompletion(MediaPlayer? _)
        {
            // throw new NotImplementedException();
        }

        bool MediaPlayer.IOnErrorListener.OnError(MediaPlayer? mp, MediaError what, int extra)
        {
            // throw new NotImplementedException();
            return false;
        }

        void MediaPlayer.IOnSeekCompleteListener.OnSeekComplete(MediaPlayer? _)
        {
            if (playOnFocusGain && null != mediaPlayer && mediaPlayer.CurrentPosition == FragmentPosition)
            {
                mediaPlayer.Start();
                State = PlaybackStateCompat.StatePlaying;
            }
        }

        private void UpdateMediaPlayerState()
        {
            if (AudioFocusState.None == audioFocusRequestor.State)
            {
                if (PlaybackStateCompat.StatePlaying == State)
                {
                    Pause();
                }
            }
            else
            {  // we have audio focus:
                if (null != mediaPlayer)
                {
                    if (AudioFocusState.CanDuck == audioFocusRequestor.State)
                    {
                        mediaPlayer.SetVolume(VolumeDuck, VolumeDuck);
                    }
                    else
                    {
                        mediaPlayer.SetVolume(VolumeNormal, VolumeNormal);
                    }
                }

                if (playOnFocusGain)
                {
                    var newState = State;

                    if (mediaPlayer is { IsPlaying: false })
                    {
                        if (mediaPlayer.CurrentPosition == FragmentPosition)
                        {
                            mediaPlayer.Start();
                            newState = PlaybackStateCompat.StatePlaying;
                            playOnFocusGain = false;
                        }
                        else
                        {
                            mediaPlayer.SeekTo(FragmentPosition, MediaPlayerSeekMode.Closest);
                            newState = PlaybackStateCompat.StateBuffering;
                        }
                    }

                    State = newState;
                }
            }
        }

        private static AudioAttributes? CreateAudioAttributes()
        {
            var builder = new AudioAttributes.Builder();
            
            builder.SetContentType(AudioContentType.Music);
            builder.SetUsage(AudioUsageKind.Media);
            builder.SetFlags(AudioFlags.None);
            
            return builder.Build();
        }

        private static AudioManager? GetAudioManager()
        {
            return AudioManager.FromContext(Application.Context);
            // return (AudioManager?)Application.Context.GetSystemService(Context.AudioService);
        }

        private void EnsureMediaPlayerCreated()
        {
            if (null != mediaPlayer)
            {
                mediaPlayer.Reset();
                return;
            }

            mediaPlayer = new MediaPlayer();
            mediaPlayer.SetWakeMode(Application.Context, WakeLockFlags.Partial);
            mediaPlayer.SetOnPreparedListener(this);
            mediaPlayer.SetOnSeekCompleteListener(this);
        }

        private void RegisterNoisyReceiver()
        {
            if (null == noisyIntent)
            {
                noisyIntent = service.RegisterReceiver(noisyReceiver, audioNoisyIntentFilter);
            }
        }

        private void UnregisterNoisyReceiver()
        {
            if (null != noisyIntent)
            {
                service.UnregisterReceiver(noisyReceiver);
                noisyIntent = null;
            }
        }
    }
}
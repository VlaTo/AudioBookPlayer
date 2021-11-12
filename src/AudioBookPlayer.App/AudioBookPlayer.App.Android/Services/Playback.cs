﻿#nullable enable

using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Support.V4.Media.Session;
using System;
using System.Threading;
using Xamarin.Forms;
using Application = Android.App.Application;
using Uri = Android.Net.Uri;

namespace AudioBookPlayer.App.Android.Services
{
    internal interface IPlaybackCallback
    {
        void StateChanged();

        void PositionChanged();

        void FragmentEnded();
    }

    internal sealed partial class Playback : Java.Lang.Object,
        MediaPlayer.IOnPreparedListener,
        MediaPlayer.IOnCompletionListener,
        MediaPlayer.IOnErrorListener,
        MediaPlayer.IOnSeekCompleteListener
    {
        private const float VolumeNormal = 1.0f;
        private const float VolumeDuck = 0.6f;

        public const float DefaultSpeed = 1.0f;

        private readonly Service service;
        private readonly MediaSessionCompat mediaSession;
        private readonly IPlaybackCallback callback;
        private readonly AudioFocusRequestor audioFocusRequestor;
        private readonly BroadcastReceiver noisyReceiver;
        private readonly IntentFilter audioNoisyIntentFilter;
        private readonly AudioAttributes? audioAttributes;
        private Intent? noisyIntent;
        private MediaPlayer? mediaPlayer;
        private Timer? playingTimer;
        private int state;
        private bool playOnFocusGain;
        private long position;

        public Uri? MediaUri
        {
            get;
            private set;
        }

        public long Offset
        {
            get;
            private set;
        }

        public long Duration
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

        public long Position
        {
            get => position;
            set
            {
                if (value == position)
                {
                    return;
                }

                position = value;
                callback.PositionChanged();
            }
        }

        public long MediaPosition => Offset + Position;

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
            Offset = -1L;
            Duration = 0L;
            Position = Offset;
        }

        public void PlayFragment(Uri mediaUri, long start, long duration, long offset = 0L)
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

            if (false == mediaHasChanged && null != mediaPlayer)
            {
                Offset = start;
                Duration = duration;
                Position = offset;

                UpdateMediaPlayerState();

                return;
            }

            MediaUri = mediaUri;
            Offset = start;
            Duration = duration;
            Position = offset;

            try
            {
                if (null != MediaUri)
                {
                    EnsureMediaPlayerCreated();

                    if (mediaHasChanged)
                    {
                        State = PlaybackStateCompat.StateBuffering;

                        mediaPlayer.SetAudioAttributes(audioAttributes);
                        mediaPlayer.SetDataSource(Application.Context, MediaUri);
                        mediaPlayer.PrepareAsync();
                    }
                    else
                    {
                        var currentPosition = (long)mediaPlayer.CurrentPosition;
                        var newPosition = Offset + Position;

                        if (currentPosition == newPosition)
                        {
                            mediaPlayer.Start();
                            playOnFocusGain = false;
                            State = PlaybackStateCompat.StatePlaying;
                        }
                        else
                        {
                            mediaPlayer.SeekTo(newPosition, MediaPlayerSeekMode.Closest);
                            State = PlaybackStateCompat.StateBuffering;
                        }
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

        public void SeekTo(long offset)
        {
            if (PlaybackStateCompat.StatePlaying == State)
            {
                if (mediaPlayer is { IsPlaying: true })
                {
                    var msec = mediaPlayer.CurrentPosition + offset;
                    mediaPlayer.SeekTo(msec, MediaPlayerSeekMode.Closest);
                }
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

                    Position = (long)mediaPlayer.CurrentPosition - Offset;
                    State = PlaybackStateCompat.StatePlaying;

                    CreatePlayTimer();
                }
            }
        }

        public void Pause()
        {
            if (PlaybackStateCompat.StatePlaying == State)
            {
                if (mediaPlayer is { IsPlaying: true })
                {
                    DeletePlayTimer();

                    mediaPlayer.Pause();
                    Position = (long)mediaPlayer.CurrentPosition - Offset;
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
                    DeletePlayTimer();

                    mediaPlayer.Stop();
                    Position = (long)mediaPlayer.CurrentPosition - Offset;
                    mediaPlayer.Dispose();
                    mediaPlayer = null;
                }

                // RelaxResources(false);
                audioFocusRequestor.Release();
            }

            State = PlaybackStateCompat.StateStopped;

            UnregisterNoisyReceiver();
        }

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
            if (playOnFocusGain && null != mediaPlayer)
            {
                if (false == mediaPlayer.IsPlaying)
                {
                    mediaPlayer.Start();
                    CreatePlayTimer();
                }

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
                    if (null != mediaPlayer)
                    {
                        var currentPosition = (long)mediaPlayer.CurrentPosition;
                        var newPosition = Offset + Position;

                        if (currentPosition == newPosition)
                        {
                            mediaPlayer.Start();
                            playOnFocusGain = false;
                            State = PlaybackStateCompat.StatePlaying;

                            CreatePlayTimer();
                        }
                        else
                        {
                            mediaPlayer.SeekTo(newPosition, MediaPlayerSeekMode.Closest);
                        }
                    }
                    else
                    {
                        ;
                    }

                    return ;
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

        private static AudioManager? GetAudioManager() => AudioManager.FromContext(Application.Context);

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

            //CreatePlayTimer();
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

        private void CreatePlayTimer()
        {
            if (null != playingTimer)
            {
                return;
            }

            playingTimer = new Timer(OnPlayingTimer, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(200.0d));

            System.Diagnostics.Debug.WriteLine("[Playback] [CreatePlayTimer] Execute");
        }

        private void DeletePlayTimer()
        {
            if (null != playingTimer)
            {
                playingTimer.Dispose();
                playingTimer = null;
                
                System.Diagnostics.Debug.WriteLine("[Playback] [DeletePlayTimer] Execute");
            }
        }

        private void OnPlayingTimer(object _)
        {
            if (null == mediaPlayer)
            {
                return;
            }

            var elapsed = mediaPlayer.CurrentPosition - Offset;

            if (Duration <= elapsed)
            {
                callback.FragmentEnded();
            }

            Position = elapsed;
        }
    }
}
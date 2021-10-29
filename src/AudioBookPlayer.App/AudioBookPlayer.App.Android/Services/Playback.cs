#nullable enable

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
        private long mediaPosition;

        public Uri? MediaUri
        {
            get;
            private set;
        }

        public long MediaStart
        {
            get;
            private set;
        }

        public long MediaDuration
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

        public long MediaPosition
        {
            get => mediaPosition;
            set
            {
                if (value == mediaPosition)
                {
                    return;
                }

                mediaPosition = value;
                callback.PositionChanged();
            }
        }

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
            MediaStart = -1L;
            MediaDuration = 0L;
            MediaPosition = MediaStart;
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
                MediaStart = start;
                MediaDuration = duration;
                MediaPosition = offset;

                UpdateMediaPlayerState();

                return;
            }

            MediaUri = mediaUri;
            MediaStart = start;
            MediaDuration = duration;
            MediaPosition = offset;

            try
            {
                if (null != MediaUri)
                {
                    EnsureMediaPlayerCreated();

                    if (null != mediaPlayer)
                    {
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
                            var position = MediaStart + MediaPosition;

                            if (currentPosition == position)
                            {
                                mediaPlayer.Start();
                                playOnFocusGain = false;
                                State = PlaybackStateCompat.StatePlaying;
                            }
                            else
                            {
                                mediaPlayer.SeekTo(position, MediaPlayerSeekMode.Closest);
                                State = PlaybackStateCompat.StateBuffering;
                            }
                        }
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
                    MediaPosition = mediaPlayer.CurrentPosition;

                    CreatePlayTimer();
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
                    DeletePlayTimer();

                    mediaPlayer.Pause();
                    MediaPosition = (long)mediaPlayer.CurrentPosition - MediaStart;
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
                    MediaPosition = (long)mediaPlayer.CurrentPosition - MediaStart;
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
                        var position = MediaStart + MediaPosition;

                        if (currentPosition == position)
                        {
                            mediaPlayer.Start();
                            playOnFocusGain = false;
                            State = PlaybackStateCompat.StatePlaying;
                        }
                        else
                        {
                            mediaPlayer.SeekTo(position, MediaPlayerSeekMode.Closest);
                            State = PlaybackStateCompat.StateBuffering;
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

            CreatePlayTimer();
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

            playingTimer = new Timer(OnPlayingTimer, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(100.0d));
        }

        private void DeletePlayTimer()
        {
            if (null != playingTimer)
            {
                playingTimer.Dispose();
                playingTimer = null;
            }
        }

        private void OnPlayingTimer(object _)
        {
            if (null == mediaPlayer)
            {
                return;
            }

            var elapsed = mediaPlayer.CurrentPosition - MediaStart;

            if (MediaDuration <= elapsed)
            {
                //Pause();
                callback.FragmentEnded();
            }

            MediaPosition = elapsed;
        }
    }
}
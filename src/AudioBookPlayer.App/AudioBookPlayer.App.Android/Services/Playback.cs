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
    internal sealed partial class Playback : Java.Lang.Object, MediaPlayer.IOnPreparedListener,
        MediaPlayer.IOnCompletionListener, MediaPlayer.IOnErrorListener, MediaPlayer.IOnSeekCompleteListener
    {
        private const float VolumeNormal = 1.0f;
        private const float VolumeDuck = 0.6f;
        private const string ModeRead = "r";

        private readonly Service service;
        private readonly IBooksService booksService;
        private readonly MediaSessionCompat mediaSession;
        private readonly AudioFocusRequestor audioFocusRequestor;
        private readonly BroadcastReceiver noisyReceiver;
        private readonly IntentFilter audioNoisyIntentFilter;
        private Intent noisyIntent;
        private MediaPlayer? mediaPlayer;
        // private string currentMediaId;
        private int state;
        private long currentPosition;
        private bool playOnFocusGain;

        public AudioBook? AudioBook
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

                StateChanged.Invoke();
            }
        }

        public bool IsConnected => true;

        public bool IsPlaying => playOnFocusGain || mediaPlayer is { IsPlaying: true };

        public long CurrentMediaPosition => null != mediaPlayer ? mediaPlayer.CurrentPosition : currentPosition;

        public Action StateChanged
        {
            get;
            set;
        }

        public Playback(Service service, MediaSessionCompat mediaSession, IBooksService booksService)
        {
            this.service = service;
            this.mediaSession = mediaSession;
            this.booksService = booksService;

            AudioBook = null;
            state = PlaybackStateCompat.StateNone;
            currentPosition = -1L;

            var audioAttributes = CreateAudioAttributes();
            var audioManager = (AudioManager?)Application.Context.GetSystemService(Context.AudioService);

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
        }

        public void Play(MediaSessionCompat.QueueItem item)
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

            var mediaId = MediaId.Parse(item.Description.MediaId);
            var mediaHasChanged = null == AudioBook || mediaId.BookId != AudioBook.Id;

            if (mediaHasChanged)
            {
                var book = booksService.GetBook(mediaId.BookId);

                AudioBook = book;
                currentPosition = 0L;
            }

            playOnFocusGain = true;

            if (PlaybackStateCompat.StatePaused == State && false == mediaHasChanged && null != mediaPlayer)
            {
                UpdateMediaPlayerState();
            }
            else
            {
                State = PlaybackStateCompat.StateStopped;

                try
                {
                    var source = GetDataSource();

                    if (null != source)
                    {
                        EnsureMediaPlayerCreated();

                        if (null != mediaPlayer)
                        {
                            State = PlaybackStateCompat.StateBuffering;

                            var attributes = CreateAudioAttributes();

                            mediaPlayer.SetAudioAttributes(attributes);
                            mediaPlayer.SetDataSource(source);
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
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    throw;
                }
            }
        }

        public bool Prepare(MediaId mediaId)
        {
            var mediaHasChanged = null == AudioBook || mediaId.BookId != AudioBook.Id;

            // State = PlaybackStateCompat.StateConnecting;

            if (mediaHasChanged)
            {
                Stop();

                var book = booksService.GetBook(mediaId.BookId);

                AudioBook = book;
                currentPosition = GetPositionFromMediaId(mediaId);
            }

            return mediaHasChanged;
        }

        public void Pause()
        {
            if (PlaybackStateCompat.StatePlaying == State)
            {
                if (null != mediaPlayer && mediaPlayer.IsPlaying)
                {
                    mediaPlayer.Pause();
                    currentPosition = mediaPlayer.CurrentPosition;
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
                    currentPosition = mediaPlayer.CurrentPosition;
                }

                // RelaxResources(false);
                audioFocusRequestor.Release();
            }

            State = PlaybackStateCompat.StateStopped;

            UnregisterNoisyReceiver();
        }

        private long GetPositionFromMediaId(MediaId mediaId)
        {
            if (null != AudioBook)
            {
                var sectionIndex = mediaId.SectionIndex.GetValueOrDefault(0);
                var chapterIndex = mediaId.ChapterIndex.GetValueOrDefault(0);

                if (AudioBook.Sections.Count > sectionIndex)
                {
                    var section = AudioBook.Sections[sectionIndex];

                    if (section.Chapters.Count > chapterIndex)
                    {
                        var chapter = section.Chapters[chapterIndex];
                        return (long)chapter.Start.TotalMilliseconds;
                    }
                }
            }

            return 0L;
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
            // throw new NotImplementedException();
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
                if (AudioFocusState.CanDuck == audioFocusRequestor.State)
                {
                    mediaPlayer.SetVolume(VolumeDuck, VolumeDuck);
                }
                else
                {
                    if (null != mediaPlayer)
                    {
                        mediaPlayer.SetVolume(VolumeNormal, VolumeNormal);
                    }
                }
                if (playOnFocusGain)
                {
                    var newState = State;

                    if (mediaPlayer != null && !mediaPlayer.IsPlaying)
                    {
                        if (mediaPlayer.CurrentPosition == currentPosition)
                        {
                            mediaPlayer.Start();
                            newState = PlaybackStateCompat.StatePlaying;
                        }
                        else
                        {
                            mediaPlayer.SeekTo(currentPosition, MediaPlayerSeekMode.Closest);
                            newState = PlaybackStateCompat.StateBuffering;
                        }
                    }

                    playOnFocusGain = false;

                    State = newState;
                }
            }

            /*if (Callback != null)
            {
                Callback.OnPlaybackStatusChanged(State);
            }*/
        }

        private static AudioAttributes? CreateAudioAttributes()
        {
            var builder = new AudioAttributes.Builder();
            
            builder.SetContentType(AudioContentType.Music);
            builder.SetUsage(AudioUsageKind.Media);
            builder.SetFlags(AudioFlags.None);
            
            return builder.Build();
        }

        private AssetFileDescriptor? GetDataSource()
        {
            // var mediaId = MediaId.Parse(currentMediaId);
            // var book = booksService.GetBook(mediaId.BookId);

            if (null != AudioBook)
            {
                var section = 0 < AudioBook.Sections.Count ? AudioBook.Sections[0] : null;

                if (null != section)
                {
                    var sourceUri = Uri.Parse(section.ContentUri);

                    if (null != sourceUri)
                    {
                        return OpenAssetFile(sourceUri);
                    }
                }
            }

            return null;
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

        private static AssetFileDescriptor? OpenAssetFile(Uri sourceUri)
        {
            var contentResolver = Application.Context.ContentResolver;
            return contentResolver?.OpenAssetFileDescriptor(sourceUri, ModeRead);
        }
    }
}
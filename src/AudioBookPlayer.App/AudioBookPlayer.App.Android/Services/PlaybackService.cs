using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Media;
using Android.OS;
using AudioBookPlayer.App.Domain.Models;
using AudioBookPlayer.App.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using Xamarin.Forms;
using Application = Android.App.Application;

namespace AudioBookPlayer.App.Android.Services
{
    // https://docs.microsoft.com/en-us/xamarin/android/app-fundamentals/android-audio
    // https://github.com/jamesmontemagno/AndroidStreamingAudio/tree/master/Part%201%20-%20Simple%20Streaming
    // https://docs.microsoft.com/en-us/xamarin/android/app-fundamentals/services/creating-a-service/bound-services
    // https://developer.android.com/reference/android/media/MediaPlayer
    [Service]
    public sealed class PlaybackService : Service, IPlaybackService
    {
        private readonly WeakEventManager eventManager;
        private readonly Dictionary<string, int> pendingEvents;
        private AudioManager audioManager;
        private AudioFocusRequestor audioFocusRequestor;
        private MediaPlayer player;
        private AudioBook audioBook;
        private AudioBookChapter chapter;
        private int chapterIndex;
        private TimeSpan currentPosition;
        private Timer playingTimer;
        private bool isPlaying;
        private int updateCount;
        private AudioAttributes audioAttributes;

        public AudioBook AudioBook
        {
            get => audioBook;
            set
            {
                if (IsPlaying)
                {
                    StopPlaying();
                }

                audioBook = value;
                chapterIndex = -1;
                CurrentPosition = TimeSpan.Zero;

                /*System.Diagnostics.Debug.WriteLine($"Book: \"{audioBook.Title}\"");
                for (var index = 0; index < audioBook.Chapters.Count; index++)
                {
                    var chapter = audioBook.Chapters[index];
                    System.Diagnostics.Debug.WriteLine($"  [{index}] \"{chapter.Title}\" ({chapter.Start:g} - {chapter.End:g})");
                }*/

                RaiseOrPostponeEvent(nameof(AudioBookChanged));
            }
        }

        public TimeSpan CurrentPosition
        {
            get => currentPosition;
            set
            {
                currentPosition = value;
                RaiseOrPostponeEvent(nameof(CurrentPositionChanged));
            }
        }

        public int ChapterIndex
        {
            get => chapterIndex;
            set
            {
                if (value < 0 || value >= audioBook.Chapters.Count)
                {
                    throw new ArgumentException("Wrong chapter index", nameof(value));
                }

                if (value == chapterIndex)
                {
                    return;
                }

                var wasPlaying = IsPlaying;

                if (wasPlaying)
                {
                    PausePlaying();
                }

                chapterIndex = value;
                chapter = audioBook.Chapters[chapterIndex];
                CurrentPosition = TimeSpan.Zero;

                System.Diagnostics.Debug.WriteLine($"[ChapterIndex] Set value: {chapterIndex}");

                RaiseOrPostponeEvent(nameof(ChapterIndexChanged));

                if (wasPlaying)
                {
                    StartPlaying();
                }
            }
        }

        public bool IsPlaying
        {
            get => isPlaying;
            set
            {
                if (isPlaying != value)
                {
                    isPlaying = value;
                    RaiseOrPostponeEvent(nameof(IsPlayingChanged));
                }
            }
        }

        public event EventHandler IsPlayingChanged
        {
            add => eventManager.AddEventHandler(value);
            remove => eventManager.RemoveEventHandler(value);
        }

        public event EventHandler AudioBookChanged
        {
            add => eventManager.AddEventHandler(value);
            remove => eventManager.RemoveEventHandler(value);
        }

        public event EventHandler ChapterIndexChanged
        {
            add => eventManager.AddEventHandler(value);
            remove => eventManager.RemoveEventHandler(value);
        }

        public event EventHandler CurrentPositionChanged
        {
            add => eventManager.AddEventHandler(value);
            remove => eventManager.RemoveEventHandler(value);
        }

        public IBinder Binder
        {
            get;
            private set;
        }

        public PlaybackService()
        {
            eventManager = new WeakEventManager();
            pendingEvents = new Dictionary<string, int>();
            updateCount = 0;
        }

        public override void OnCreate()
        {
            audioBook = null;
            chapter = null;
            currentPosition = TimeSpan.Zero;

            base.OnCreate();

            audioAttributes = new AudioAttributes.Builder().SetContentType(AudioContentType.Music).Build();
            audioManager = (AudioManager) Application.Context.GetSystemService(Context.AudioService);
            audioFocusRequestor = new AudioFocusRequestor(audioManager, audioAttributes);
        }

        public override IBinder OnBind(Intent intent)
        {
            Binder = new PlaybackServiceBinder(this);

            return Binder;
        }

        public override bool OnUnbind(Intent intent)
        {
            return base.OnUnbind(intent);
        }

        public override void OnDestroy()
        {
            audioManager = null;
            chapter = null;
            audioBook = null;

            if (player.IsPlaying)
            {
                player.Stop();
            }

            audioFocusRequestor.Release();

            player = null;
            Binder = null;

            base.OnDestroy();
        }

        public void Play()
        {
            if (null == audioBook)
            {
                throw new Exception();
            }

            if (0 > chapterIndex)
            {
                throw new Exception();
            }

            if (audioBook.Chapters.Count <= chapterIndex)
            {
                throw new Exception();
            }

            if (chapter.Duration <= currentPosition)
            {
                throw new Exception();
            }

            EnsurePlayerInitialized();

            if (OpenDataSource())
            {
                StartPlaying();
            }
        }

        public void Pause()
        {
            PausePlaying();
        }

        public IDisposable BatchUpdate()
        {
            updateCount++;
            return new UpdateToken(this);
        }

        private void EnsurePlayerInitialized()
        {
            if (null != player)
            {
                return;
            }

            player = new MediaPlayer();

            player.SetAudioAttributes(audioAttributes);
            player.SetWakeMode(Application.Context, WakeLockFlags.Partial);
            player.AudioSessionId = audioManager.GenerateAudioSessionId();
        }

        private bool OpenDataSource()
        {
            var fragment = GetChapterFragment(currentPosition, chapter);

            if (null == fragment)
            {
                return false;
            }

            var descriptor = OpenFileDescriptor(fragment);

            player.Reset();
            player.SetDataSource(descriptor);

            if (AudioFocusRequest.Granted != audioFocusRequestor.Acquire())
            {
                System.Diagnostics.Debug.WriteLine($"[AndroidPlaybackService] [StartPlayFile] Audio focus not acquired!");
                return false;
            }

            player.Prepare();

            return true;
        }

        private void StartPlaying()
        {
            var offset = (chapter.Start + currentPosition).TotalMilliseconds;
            
            player.SeekTo((long)offset, MediaPlayerSeekMode.Closest);
            player.Start();

            if (player.IsPlaying != IsPlaying)
            {
                IsPlaying = player.IsPlaying;

                if (IsPlaying)
                {
                    playingTimer = new Timer(OnTimer, null, TimeSpan.Zero, TimeSpan.FromSeconds(1.0d));
                }
            }
        }

        private void PausePlaying()
        {
            if (false == IsPlaying)
            {
                return;
            }

            player.Pause();

            if (null != playingTimer)
            {
                playingTimer.Dispose();
                playingTimer = null;
            }

            IsPlaying = player.IsPlaying;
        }

        private void StopPlaying()
        {
            if (false == IsPlaying)
            {
                return;
            }

            player.Stop();

            if (null != playingTimer)
            {
                playingTimer.Dispose();
                playingTimer = null;
            }

            IsPlaying = player.IsPlaying;
        }


        private void EndUpdate()
        {
            if (0 < updateCount)
            {
                if (0 == --updateCount)
                {
                    RaisePendingEvents();
                }
            }
        }

        private void RaisePendingEvents()
        {
            RaiseAndRemovePendingEvent(nameof(AudioBookChanged));
            RaiseAndRemovePendingEvent(nameof(ChapterIndexChanged));
            RaiseAndRemovePendingEvent(nameof(CurrentPositionChanged));
        }

        private void RaiseAndRemovePendingEvent(string eventName)
        {
            if (pendingEvents.TryGetValue(eventName, out var count))
            {
                if (0 == count)
                {
                    return;
                }

                eventManager.HandleEvent(this, EventArgs.Empty, eventName);
                pendingEvents.Remove(eventName);
            }
        }

        private void RaiseOrPostponeEvent(string eventName)
        {
            if (0 == updateCount)
            {
                eventManager.HandleEvent(this, EventArgs.Empty, eventName);
                return;
            }

            if (false == pendingEvents.TryGetValue(eventName, out var count))
            {
                count = 0;
            }

            pendingEvents[eventName] = ++count;
        }

        private static AudioBookChapterFragment GetChapterFragment(TimeSpan position, AudioBookChapter chapter)
        {
            for (var index = 0; index < chapter.Fragments.Count; index++)
            {
                var fragment = chapter.Fragments[index];

                if (fragment.Duration > position)
                {
                    return fragment;
                }

                position -= fragment.Duration;
            }

            return null;
        }

        private static AssetFileDescriptor OpenFileDescriptor(AudioBookChapterFragment fragment)
        {
            var contentResolver = Application.Context.ContentResolver;
            var source = fragment.SourceFile;
            var uri = global::Android.Net.Uri.Parse(source.ContentUri);

            return contentResolver.OpenAssetFileDescriptor(uri, "r");
        }

        private void OnTimer(object state)
        {
            var position = TimeSpan.FromMilliseconds(player.CurrentPosition);

            if (null != chapter)
            {
                //var value = position - chapter.Start;

                if (position > chapter.End)
                {
                    chapterIndex++;
                    chapter = audioBook.Chapters[chapterIndex];

                    RaiseOrPostponeEvent(nameof(ChapterIndexChanged));
                }

                position -= chapter.Start;
            }

            CurrentPosition = position;
        }

        /*public async Task PlayAsync(
            System.IO.Stream stream,
            string audioEncoding,
            int sampleRate,
            string audioChannels)
        {
            var player = new MediaPlayer();



            var mediaEncoding = GetAudioEncoding(audioEncoding);// Encoding.Mp3;
            var channels = GetAudioChannels(audioChannels);// ChannelOut.Stereo;
            var audioAttributes = new AudioAttributes.Builder()
                    .SetFlags(AudioFlags.None)
                    .SetContentType(AudioContentType.Music)
                    .SetUsage(AudioUsageKind.Media)
                    .Build();
            var audioFormat = new AudioFormat.Builder()
                .SetEncoding(mediaEncoding)
                .SetSampleRate(sampleRate)
                .SetChannelMask(channels)
                .Build();

            var bufferSize = AudioTrack.GetMinBufferSize(sampleRate, channels, mediaEncoding);

            System.Diagnostics.Debug.WriteLine($"[PlaybackTest] [PlayAsync] Trying to allocate buffer of {bufferSize} bytes");

            using (var audio = new AudioTrack(audioAttributes, audioFormat, bufferSize, AudioTrackMode.Stream, 1))
            {
                audio.SetVolume(1.0f);

                //audio.SetNotificationMarkerPosition((int)streamLength / 2);
                //audio.SetPlaybackPositionUpdateListener(this);
                //audio.SetPositionNotificationPeriod(10);

                audio.Play();

                var buffer = new byte[bufferSize];

                while (true)
                {
                    var count = await stream.ReadAsync(buffer);

                    if (0 == count)
                    {
                        break;
                    }

                    await audio.WriteAsync(buffer, 0, count);
                }

                //audio.SetPositionNotificationPeriod(0);
                //audio.SetPlaybackPositionUpdateListener(null);

                System.Diagnostics.Debug.WriteLine("[PlaybackTest] [PlayAsync] Done playing");

                audio.Release();
            }
        }*/

        /*private static Encoding GetAudioEncoding(string value)
        {
            switch (value)
            {
                case "Encoding.Mp3":
                {
                    return Encoding.Mp3;
                }

                case "Encoding.Aac":
                {
                    return Encoding.AacHeV1;
                }

                case "Encoding.Ac3":
                {
                    return Encoding.Ac3;
                }

                case "Encoding.Pcm16":
                {
                    return Encoding.Pcm16bit;
                }

                case "Encoding.Pcm24":
                {
                    return Encoding.PcmFloat;
                }
            }

            throw new NotSupportedException();
        }

        private static ChannelOut GetAudioChannels(string value)
        {
            switch (value)
            {
                case "ChannelOut.Stereo":
                {
                    return ChannelOut.Stereo;
                }

                case "ChannelOut.Mono":
                {
                    return ChannelOut.Mono;
                }
            }

            throw new NotSupportedException();
        }*/

        private sealed class UpdateToken : IDisposable
        {
            private readonly PlaybackService service;
            private bool disposed;

            public UpdateToken(PlaybackService service)
            {
                this.service = service;
            }

            public void Dispose()
            {
                if (disposed)
                {
                    return;
                }

                try
                {
                    service.EndUpdate();
                }
                finally
                {
                    disposed = true;
                }
            }
        }
    }
}
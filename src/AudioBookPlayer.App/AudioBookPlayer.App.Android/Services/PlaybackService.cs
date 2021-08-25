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
using Uri = global::Android.Net.Uri;

namespace AudioBookPlayer.App.Android.Services
{
    // https://docs.microsoft.com/en-us/xamarin/android/app-fundamentals/android-audio
    // https://github.com/jamesmontemagno/AndroidStreamingAudio/tree/master/Part%201%20-%20Simple%20Streaming
    // https://docs.microsoft.com/en-us/xamarin/android/app-fundamentals/services/creating-a-service/bound-services
    // https://developer.android.com/reference/android/media/MediaPlayer
    [Service]
    public sealed partial class PlaybackService : Service, IPlaybackService
    {
        private readonly WeakEventManager eventManager;
        private readonly Dictionary<string, int> pendingEvents;

        private AudioManager audioManager;
        private AudioFocusRequestor audioFocusRequestor;
        private AudioAttributes audioAttributes;
        private MediaPlayer player;
        private AudioBook audioBook;
        private AudioBookChapter chapter;
        private AudioBookChapterFragment fragment;
        private PlayerState currentState;
        private string currentSourceUri;
        private int chapterIndex;
        private int fragmentIndex;
        private TimeSpan currentPosition;
        private Timer playingTimer;
        private bool isPlaying;
        private int updateCount;

        public AudioBook AudioBook
        {
            get => audioBook;
            set => state.SetAudioBook(value);
            /*{
                if (ReferenceEquals(audioBook, value))
                {
                    return;
                }

                if (IsPlaying)
                {
                    StopPlaying();
                }

                audioBook = value;

                RaiseOrPostponeEvent(nameof(AudioBookChanged));

                if (0 == audioBook.Chapters.Count)
                {
                    ChapterIndex = -1;

                    return;
                }
                
                ChapterIndex = 0;
                CurrentPosition = TimeSpan.Zero;
            }*/
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
            set => state.SetChapterIndex(value);
            /*{
                if (chapterIndex == value)
                {
                    return;
                }

                if (0 > value || value >= audioBook.Chapters.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "");
                }

                var wasPlaying = IsPlaying;

                if (wasPlaying)
                {
                    PausePlaying();
                }

                chapterIndex = value;
                chapter = audioBook.Chapters[value];
                
                if (0 == chapter.Fragments.Count)
                {
                    fragmentIndex = -1;
                    fragment = null;

                    return;
                }

                FragmentIndex = 0;

                RaiseOrPostponeEvent(nameof(ChapterIndexChanged));

                if (wasPlaying)
                {
                    StartPlaying();
                }
            }*/
        }

        public bool IsPlaying
        {
            get => isPlaying;
            private set
            {
                if (isPlaying == value)
                {
                    return;
                }

                isPlaying = value;

                RaiseOrPostponeEvent(nameof(IsPlayingChanged));
            }
        }

        internal int FragmentIndex
        {
            get => fragmentIndex;
            private set
            {
                if (0 > value || value >= chapter.Fragments.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "");
                }

                fragmentIndex = value;
                fragment = chapter.Fragments[fragmentIndex];
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
            state = null;
            updateCount = 0;
        }

        public override void OnCreate()
        {
            audioBook = null;
            chapter = null;
            fragment = null;
            chapterIndex = -1;
            fragmentIndex = -1;

            State = new NoInitializedState(this);

            base.OnCreate();

            audioAttributes = new AudioAttributes.Builder().SetContentType(AudioContentType.Music).Build();
            audioManager = (AudioManager) Application.Context.GetSystemService(Context.AudioService);
            audioFocusRequestor = new AudioFocusRequestor(audioManager, audioAttributes, OnAudioFocusChanged);
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
            fragment = null;
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

        public void Play() => state.Play();

        public void Pause() => state.Pause();

        public IDisposable BatchUpdate()
        {
            updateCount++;
            return new UpdateToken(this);
        }

        private void InitializePlayer()
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
            var sourceUri = fragment.SourceFile.ContentUri;
            var descriptor = OpenFileDescriptor(Uri.Parse(sourceUri));

            player.Reset();
            player.SetDataSource(descriptor);

            currentSourceUri = sourceUri;

            if (AudioFocusRequest.Granted != audioFocusRequestor.Acquire())
            {
                return false;
            }

            player.Prepare();

            return true;
        }

        private void StartPlayingInternal(bool forceReset)
        {
            if (forceReset)
            {
                var offset = (long) (fragment.Start + currentPosition).TotalMilliseconds;
                //var offset = (long) fragment.Start.TotalMilliseconds;
                player.SeekTo(offset, MediaPlayerSeekMode.Closest);
            }

            player.Start();

            if (player.IsPlaying)
            {
                playingTimer = new Timer(OnTimer, null, TimeSpan.Zero, TimeSpan.FromSeconds(1.0d));
                IsPlaying = true;
            }

            RaiseOrPostponeEvent(nameof(IsPlayingChanged));
        }

        private void PausePlayingInternal()
        {
            player.Pause();

            if (null != playingTimer)
            {
                playingTimer.Dispose();
                playingTimer = null;
                IsPlaying = false;
            }

            RaiseOrPostponeEvent(nameof(IsPlayingChanged));
        }

        private void StopPlayingInternal()
        {
            player.Stop();

            if (null != playingTimer)
            {
                playingTimer.Dispose();
                playingTimer = null;
                IsPlaying = false;
            }

            RaiseOrPostponeEvent(nameof(IsPlayingChanged));
        }

        private bool SetAudioBookInternal(AudioBook value)
        {
            if (ReferenceEquals(audioBook, value))
            {
                return false;
            }

            audioBook = value;

            RaiseOrPostponeEvent(nameof(AudioBookChanged));

            return true;
        }

        private void ResetChapterIndexInternal()
        {
            chapterIndex = 0 < audioBook.Chapters.Count ? 0 : -1;
            chapter = 0 == chapterIndex ? audioBook.Chapters[chapterIndex] : null;
            fragmentIndex = (null != chapter && 0 < chapter.Fragments.Count) ? 0 : -1;
            fragment = 0 == fragmentIndex ? chapter.Fragments[fragmentIndex] : null;

            RaiseOrPostponeEvent(nameof(ChapterIndexChanged));
        }

        private bool SetChapterIndexInternal(int value)
        {
            if (0 > value || value >= audioBook.Chapters.Count)
            {
                return false;
            }

            chapterIndex = value;
            chapter = -1 < chapterIndex ? audioBook.Chapters[chapterIndex] : null;
            fragmentIndex = (null != chapter && 0 < chapter.Fragments.Count) ? 0 : -1;
            fragment = 0 == fragmentIndex ? chapter.Fragments[fragmentIndex] : null;

            RaiseOrPostponeEvent(nameof(ChapterIndexChanged));

            return true;
        }

        private bool HasSourceUriChanged() => null != fragment && currentSourceUri != fragment.SourceFile.ContentUri;

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

        private void OnAudioFocusChanged(AudioFocus focusChange)
        {
            switch (focusChange)
            {
                case AudioFocus.Gain:
                {
                    System.Diagnostics.Debug.WriteLine($"[AndroidPlaybackService] [OnAudioFocusChange] Start playing");
                    break;
                }

                case AudioFocus.Loss:
                {
                    System.Diagnostics.Debug.WriteLine($"[AndroidPlaybackService] [OnAudioFocusChange] Stop playing");
                    break;
                }

                case AudioFocus.LossTransient:
                {
                    System.Diagnostics.Debug.WriteLine($"[AndroidPlaybackService] [OnAudioFocusChange] Payse playing");
                    break;
                }

                case AudioFocus.LossTransientCanDuck:
                {
                    System.Diagnostics.Debug.WriteLine($"[AndroidPlaybackService] [OnAudioFocusChange] Mute playing");
                    break;
                }
            }
        }

        /*private bool NextFragment()
        {
            var index = currentFragmentIndex + 1;

            if (index >= currentChapter.Fragments.Count)
            {
                return false;
            }

            currentFragmentIndex = index;
            currentFragment = currentChapter.Fragments[index];

            return true;
        }*/

        /*private bool NextChapter()
        {
            var index = currentChapterIndex;

            //if()
            return false;
        }

        private static int GetChapterFragmentIndex(AudioBookChapter chapter, TimeSpan position)
        {
            if (chapter.Duration >= position)
            {
                for (var index = 0; index < chapter.Fragments.Count; index++)
                {
                    var fragment = chapter.Fragments[index];

                    if (fragment.Duration > position)
                    {
                        return index;
                    }

                    position -= fragment.Duration;
                }
            }

            return -1;
        }*/

        /*private AudioBookChapterFragment GetChapterFragment(TimeSpan position, AudioBookChapter chapter)
        {
            for (var index = 0; index < chapter.Fragments.Count; index++)
            {
                var fragment = chapter.Fragments[index];

                if (fragment.Duration > position)
                {
                    currentFragmentIndex
                    return fragment;
                }

                position -= fragment.Duration;
            }

            return null;
        }*/

        //private Uri GetSourceFileUri() => Uri.Parse(fragment.SourceFile.ContentUri);

        private static AssetFileDescriptor OpenFileDescriptor(Uri contentUri)
        {
            const string readMode = "r";

            var contentResolver = Application.Context.ContentResolver;

            if (null != contentUri && null != contentResolver)
            {
                return contentResolver.OpenAssetFileDescriptor(contentUri, readMode);
            }

            return null;
        }

        private bool IsSameSource(Uri first, Uri second)
        {
            return first == second;
        }
        
        private bool NextFragment(TimeSpan position)
        {
            for (var index = fragmentIndex; index < chapter.Fragments.Count; index++)
            {
                var chapterFragment = chapter.Fragments[index];

                if (position > chapterFragment.End)
                {
                    continue;
                }

                FragmentIndex = index;

                return true;
            }

            return false;
        }
        
        private bool NextChapter(TimeSpan position)
        {
            for (var index = chapterIndex; index < audioBook.Chapters.Count; index++)
            {
                var bookChapter = audioBook.Chapters[index];

                if (position > bookChapter.End)
                {
                    continue;
                }

                ChapterIndex = index;

                return true;
            }

            return false;
        }

        private void OnTimer(object _)
        {
            var position = TimeSpan.FromMilliseconds(player.CurrentPosition);

            if (position > fragment.End)
            {
                if (NextFragment(position))
                {
                    //ContinuePlaying();
                }
                else
                if (NextChapter(position))
                {
                    //ContinuePlaying();
                }
                else
                if (position >= audioBook.Duration)
                {
                    StopPlayingInternal();
                    return;
                }
            }

            //CurrentPosition = position - chapter.Start;
            CurrentPosition = position - fragment.Start;
        }

        /*private void ContinuePlaying()
        {
            var sourceUri = GetSourceFileUri();

            if (IsSameSource(sourceUri, currentSourceUri))
            {
                return;
            }

            if (OpenDataSource())
            {
                StartPlayingInternal(false);
            }
        }*/

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

        // AudioBookData
        /*private sealed class AudioBookData
        {
            private readonly PlaybackService playbackService;

            private AudioBook audioBook;
            private AudioBookChapter chapter;
            private AudioBookChapterFragment fragment;
            private int chapterIndex;
            private int fragmentIndex;

            public AudioBook AudioBook
            {
                get => audioBook;
                set
                {
                    if (ReferenceEquals(audioBook, value))
                    {
                        return;
                    }

                    audioBook = value;

                    if (0 == audioBook.Chapters.Count)
                    {
                        return;
                    }

                    ChapterIndex = 0;
                    playbackService.CurrentPosition = TimeSpan.Zero;
                }
            }

            public AudioBookChapter Chapter => chapter;

            public int ChapterIndex
            {
                get => chapterIndex;
                set
                {
                    if (chapterIndex == value)
                    {
                        return;
                    }

                    if (0 > value || value >= chapterIndex)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), "");
                    }

                    chapterIndex = value;
                    chapter = audioBook.Chapters[value];

                    if (0 == chapter.Fragments.Count)
                    {
                        fragmentIndex = -1;
                        fragment = null;

                        return;
                    }

                    FragmentIndex = 0;
                }
            }

            public AudioBookChapterFragment Fragment => fragment;

            public int FragmentIndex
            {
                get => fragmentIndex;
                set
                {
                    if (fragmentIndex == value)
                    {
                        return;
                    }

                    if (0 > value || value >= fragmentIndex)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), "");
                    }

                    fragmentIndex = 0;
                    fragment = chapter.Fragments[fragmentIndex];
                }
            }

            public AudioBookData(PlaybackService playbackService)
            {
                this.playbackService = playbackService;
                audioBook = null;
                chapter = null;
                fragment = null;
                chapterIndex = -1;
                fragmentIndex = -1;
            }

            public void Reset()
            {
                ;
            }

            public bool IsPositionOverflow(TimeSpan position)
            {
                return null != audioBook && position > fragment.End;
            }

            public void UpdatePosition(TimeSpan position)
            {
                if (NextFragment(position))
                {
                    OpenMedia();
                }
                else
                if (NextChapter(position))
                {
                    OpenMedia();
                }
                else
                if (IsBookEnd(position))
                {
                    return;
                }

                playbackService.CurrentPosition = position - chapter.Start;
            }

            public long GetCurrentOffset()
            {
                return (long) (fragment.Start + playbackService.CurrentPosition).TotalMilliseconds;
            }

            public AssetFileDescriptor OpenFileDescriptor()
            {
                const string readMode = "r";

                var contentResolver = Application.Context.ContentResolver;
                var source = fragment.SourceFile;
                var uri = Uri.Parse(source.ContentUri);

                if (null != uri && null != contentResolver)
                {
                    return contentResolver.OpenAssetFileDescriptor(uri, readMode);
                }

                return null;
            }

            private void OpenMedia()
            {
            }

            private bool NextFragment(TimeSpan position)
            {
                for (var index = fragmentIndex; index < chapter.Fragments.Count; index++)
                {
                    var chapterFragment = chapter.Fragments[index];

                    if (position > chapterFragment.End)
                    {
                        continue;
                    }

                    FragmentIndex = index;

                    return true;
                }

                return false;
            }

            private bool NextChapter(TimeSpan position)
            {
                for (var index = chapterIndex; index < audioBook.Chapters.Count; index++)
                {
                    var bookChapter = audioBook.Chapters[index];

                    if (position > bookChapter.End)
                    {
                        continue;
                    }

                    ChapterIndex = index;

                    return true;
                }

                return false;
            }

            private bool IsBookEnd(TimeSpan position)
            {
                return position >= audioBook.Duration;
            }
        }*/
    }
}
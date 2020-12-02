using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;

namespace AudioBookPlayer.App.Droid.Services
{
    // https://docs.microsoft.com/en-us/xamarin/android/app-fundamentals/android-audio
    // https://github.com/jamesmontemagno/AndroidStreamingAudio/tree/master/Part%201%20-%20Simple%20Streaming
    [Service]
    [IntentFilter(new []{ ActionPlay })]
    internal sealed class AndroidPlaybackService : Service, AudioManager.IOnAudioFocusChangeListener
    {
        public const string ActionPlay = "com.libraprogramming.audiobookreader.action.play";

        private AudioManager audioManager;
        private MediaPlayer player;

        public AndroidPlaybackService()
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            audioManager = (AudioManager)GetSystemService(AudioService);

        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(
            Intent intent,
            [GeneratedEnum] StartCommandFlags flags,
            int startId)
        {
            switch (intent.Action)
            {
                case ActionPlay:
                {
                    var filename = intent.GetStringExtra("Filename");
                    
                    System.Diagnostics.Debug.WriteLine($"[AndroidPlaybackService] [OnStartCommand] Action: '{ActionPlay}', Filenamw: '{filename}'");

                    StartPlayFile(filename);

                    break;
                }
            }

            return StartCommandResult.Sticky;
        }

        void AudioManager.IOnAudioFocusChangeListener.OnAudioFocusChange([GeneratedEnum] AudioFocus focusChange)
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

        private void StartPlayFile(string filename)
        {
            if (null == player)
            {
                player = new MediaPlayer();
                player.SetAudioStreamType(Stream.Music);
                player.SetWakeMode(ApplicationContext, WakeLockFlags.Partial);
            }
            else
            {
                player.Reset();
            }

            player.SetDataSource(filename);

            var result = audioManager.RequestAudioFocus(this, Stream.Music, AudioFocus.Gain);

            if (AudioFocusRequest.Granted != result)
            {
                System.Diagnostics.Debug.WriteLine($"[AndroidPlaybackService] [StartPlayFile] Audio focus not accuired!");
                return;
            }

            player.Prepare();
            player.Start();
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
    }
}
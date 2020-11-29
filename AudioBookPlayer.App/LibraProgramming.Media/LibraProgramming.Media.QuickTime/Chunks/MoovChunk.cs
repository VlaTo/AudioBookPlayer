using LibraProgramming.Media.QuickTime.Components;
using System;
using System.Collections.Generic;

namespace LibraProgramming.Media.QuickTime.Chunks
{
    [Chunk(AtomTypes.Moov)]
    internal sealed class MoovChunk : ContainerChunk
    {
        private MoovChunk(Chunk[] chunks)
            : base(AtomTypes.Moov, chunks)
        {
        }

        [ChunkCreator]
        public static new MoovChunk ReadFrom(Atom atom)
        {
            if (null == atom)
            {
                throw new ArgumentNullException(nameof(atom));
            }

            var chunks = new List<Chunk>();

            using (var source = new ReadOnlyAtomStream(atom.Stream, 0, atom.Stream.Length))
            {
                var extractor = new AtomExtractor(atom.Stream);

                foreach (var child in extractor)
                {
                    var chunk = ChunkFactory.Instance.CreateFrom(child);

                    switch (chunk)
                    {
                        case MvhdChunk mvhd:
                        {

                            break;
                        }

                        case TrakChunk track:
                        {

                            break;
                        }

                        case UdtaChunk udta:
                        {

                            break;
                        }

                        default:
                        {

                            break;
                        }
                    }

                    chunks.Add(chunk);
                }
            }

            return new MoovChunk(chunks.ToArray());
        }

        /*public new static MoovChunk ReadFrom(Atom atom)
        {
            var chunks = new List<Chunk>();

            using (var extractor = new AtomExtractor(atom.Stream))
            {
                var factory = ChunkFactory.Instance;

                foreach (var child in extractor)
                {
                    var chunk = factory.CreateFrom(child);
                    chunks.Add(chunk);
                }
            }

            return new MoovChunk(chunks.ToArray());
            

            >>var offset = header.Offset + header.Length;
            var tracks = new List<TrackInfo>();

            ReadChunk(stream, offset, header.ChunkLength, 
                (arg1, arg2, arg3, arg4) => ReadBox(arg1, arg2, arg3, arg4, tracks)
            );

            var soundBoxIndex = tracks.FindIndex(box => BoxType.Soun.Equals(box.Type));

            if (soundBoxIndex >= 0)
            {
                var soundBox = tracks[soundBoxIndex];
                var textBoxIndex = tracks.FindIndex(box =>
                    BoxType.Text.Equals(box.Type) && Array.Exists(soundBox.Chapters, index => box.Id == index)
                );

                if (textBoxIndex >= 0)
                {
                    var encoding = new UnicodeEncoding(false, false);
                    var textBox = tracks[textBoxIndex];

                    textBox.Titles = new string[textBox.Samples.Length];

                    for (var index = 0; index < textBox.Samples.Length; index++)
                    {
                        var sample = textBox.Samples[index];
                        var position = stream.Seek(sample, SeekOrigin.Begin);
                        var length = StreamHelper.ReadUInt16(stream);
                        var span = StreamHelper.ReadBytes(stream, length);

                        textBox.Titles[index] = encoding.GetString(span);
                    }
                }
            }<<
        }

        public override void Debug(int level)
        {
            var tabs = new String(' ', level);
            var bytes = BitConverter.GetBytes(Type).ToBigEndian();
            var type = Encoding.ASCII.GetString(bytes);

            Console.WriteLine($"{tabs}{type}");

            base.Debug(level);

            >>foreach (var track in tracks)
            {
                Console.WriteLine($"  Track: '{track.Type}', id: {track.Id}");
                Console.WriteLine("    Chapters");

                for (var index = 0; index < track.Chapters.Length; index++)
                {
                    var chapter = track.Chapters[index];
                    Console.WriteLine($"    [{index:D3}]: id: {chapter}");
                }

                Console.WriteLine("    Durations");
                var position = TimeSpan.Zero;

                for (var index = 0; index < track.Durations.Length; index++)
                {
                    var length = track.Durations[index];
                    var duration = TimeSpan.FromSeconds((double) length / track.TimeUnitPerSecond);
                    Console.WriteLine($"    [{index:D3}]: length: {length}, start: {position:c}, duration: {duration}");
                    position += duration;
                }

                Console.WriteLine($"    Total duration: {position:c}");

                Console.WriteLine("    Samples");

                for (var index = 0; index < track.Samples.Length; index++)
                {
                    var offset = track.Samples[index];
                    Console.WriteLine($"    [{index:D3}]: offset: {offset:X8}");
                }

                Console.WriteLine("    Titles");
                var position = TimeSpan.Zero;

                for (var index = 0; index < track.Titles.Length; index++)
                {
                    var title = track.Titles[index];
                    var length = track.Durations[index];
                    var duration = TimeSpan.FromSeconds((double)length / track.TimeUnitPerSecond);

                    Console.WriteLine($"    [{index:D3}]: title: \"{title}\", start: {position:c}, duration: {duration}");

                    position += duration;
                }

                Console.WriteLine("    Samples");

                for (var index = 0; index < track.Samples.Length; index++)
                {
                    var offset = track.Samples[index];
                    Console.WriteLine($"    [{index:D3}]: offset: {offset:X8}");
                }
            }<<

            >>var soundBoxIndex = tracks.FindIndex(box => BoxType.Soun.Equals(box.Type));

            if (soundBoxIndex >= 0)
            {
                var soundBox = tracks[soundBoxIndex];
                var textBoxIndex = tracks.FindIndex(box =>
                    BoxType.Text.Equals(box.Type) && Array.Exists(soundBox.Chapters, index => box.Id == index)
                );

                if (textBoxIndex >= 0)
                {
                    var trackInfo = tracks[textBoxIndex];
                    var position = TimeSpan.Zero;
                    var timeUnits = trackInfo.TimeUnitPerSecond;

                    Console.WriteLine("    Titles");

                    for (var index = 0; index < trackInfo.Titles.Length; index++)
                    {
                        var title = trackInfo.Titles[index];
                        var length = trackInfo.Durations[index];
                        var offset = trackInfo.Samples[index];
                        var frameCount = trackInfo.FrameCount[index];
                        var duration = TimeSpan.FromSeconds((double) length / timeUnits);

                        Console.WriteLine($"    [{index:D3}]: title: \"{title}\", start: {position:c}, duration: {duration}, length: {length:D} sample: {offset:X8}, frames: {frameCount:D}");

                        position += duration;
                    }

                    //Console.WriteLine($"  SoundBox: '{soundBox.Type}', id: {soundBox.Id}");
                }
            }
            else
            {
                Console.WriteLine("No soundbox");
            }<<
        }

        >>private static void ReadBox(Stream stream, BoxType type, long offset, uint length, List<TrackInfo> trackInfos)
        {
            var boxOffset = offset + StreamHelper.PrefixSize;
            var boxLength = length - StreamHelper.PrefixSize;

            if (type.Equals(BoxType.Mvhd))
            {
                ;
            }
            else if(type.Equals(BoxType.Trak))
            {
                var trackInfo = new TrackInfo();

                ReadChunk(stream, boxOffset, boxLength, 
                    (arg1, arg2, arg3, arg4) => ReadTrack(arg1, arg2, arg3, arg4, trackInfo)
                );

                if (false == trackInfo.Empty && trackInfo.IsValid())
                {
                    trackInfos.Add(trackInfo);
                }
            }

            >>
                case "iods":
                case "udta":
                case "free":
            <<
        }

        private static void ReadTrack(Stream stream, BoxType type, long offset, uint length, TrackInfo trackInfo)
        {
            var boxOffset = offset + StreamHelper.PrefixSize;
            var boxLength = length - StreamHelper.PrefixSize;

            if (type.Equals(BoxType.Tkhd))
            {
                ReadTkhd(stream, trackInfo);
            }

            if (type.Equals(BoxType.Mdia))
            {
                ReadChunk(stream, boxOffset, boxLength, 
                    (arg1, arg2, arg3, arg4) => ReadMdia(arg1, arg2, arg3, arg4, trackInfo)
                );
            }

            if (type.Equals(BoxType.Tref))
            {
                ReadChunk(stream, boxOffset, boxLength, 
                    (arg1, arg2, arg3, arg4) => ReadTref(arg1, arg2, arg3, arg4, trackInfo)
                );
            }

            >>
                case "udta":
                case "trak":
            <<
        }

        private static void ReadMdia(Stream stream, BoxType type, long offset, uint length, TrackInfo trackInfo)
        {
            var boxOffset = offset + StreamHelper.PrefixSize;
            var boxLength = length - StreamHelper.PrefixSize;

            if (type.Equals(BoxType.Mdhd))
            {
                ReadMdhd(stream, trackInfo);
            }
            else if (type.Equals(BoxType.Minf))
            {
                ReadChunk(stream, boxOffset, boxLength, 
                    (arg1, arg2, arg3, arg4) => ReadMinf(arg1, arg2, arg3, arg4, trackInfo)
                );
            }
            else if (type.Equals(BoxType.Hdlr))
            {
                ReadHdlr(stream, trackInfo);
            }

            >>
                case "udta":
            <<
        }

        private static void ReadMinf(Stream stream, BoxType type, long offset, uint length, TrackInfo trackInfo)
        {
            var boxOffset = offset + StreamHelper.PrefixSize;
            var boxLength = length - StreamHelper.PrefixSize;

            if (type.Equals(BoxType.Stbl))
            {
                ReadChunk(stream, boxOffset, boxLength, 
                    (arg1, arg2, arg3, arg4) => ReadStbl(arg1, arg2, arg3, arg4, trackInfo)
                );
            }

            >>
                case "udta":
                case "gmhd":
                case "dinf":
                case "smhd":
            <<
        }

        private static void ReadTref(Stream stream, BoxType type, long offset, uint length, TrackInfo trackInfo)
        {
            if (type.Equals(BoxType.Chap))
            {
                ReadChap(stream, length, trackInfo);
            }

            >>
                case "udta":
            <<
        }

        private static void ReadStbl(Stream stream, BoxType type, long offset, uint length, TrackInfo trackInfo)
        {
            if (type.Equals(BoxType.Stco))
            {
                ReadStco(stream, trackInfo);
            }
            else if(type.Equals(BoxType.Stts))
            {
                ReadStts(stream, trackInfo);
            }

            // stsd
            // stsz
            // stsc
            // ctts
        }

        private static void ReadTkhd(Stream stream, TrackInfo trackInfo)
        {
            var flag = StreamHelper.ReadByte(stream);
            var isv8 = 1 == flag;
            var count = stream.Seek(3 + (isv8 ? 8 + 8 : 4 + 4), SeekOrigin.Current);

            trackInfo.Id = StreamHelper.ReadUInt32(stream);
        }

        private static void ReadMdhd(Stream stream, TrackInfo trackInfo)
        {
            var flag = StreamHelper.ReadByte(stream);
            var isv8 = 1 == flag;
            var count = stream.Seek(3 + (isv8 ? 8 + 8 : 4 + 4), SeekOrigin.Current);

            trackInfo.TimeUnitPerSecond = StreamHelper.ReadUInt32(stream);
        }

        private static void ReadHdlr(Stream stream, TrackInfo trackInfo)
        {
            stream.Seek(4 + 4, SeekOrigin.Current);

            var span = StreamHelper.ReadBytes(stream, 4);

            trackInfo.Type = BoxType.From(span);
        }

        private static void ReadStco(Stream stream, TrackInfo trackInfo)
        {
            stream.Seek(4, SeekOrigin.Current);

            var count = StreamHelper.ReadUInt32(stream);

            if (count > 1024)
            {
                count = 0;
            }

            trackInfo.Samples = new long[count];

            for (var index = 0; index < count; index++)
            {
                trackInfo.Samples[index] = StreamHelper.ReadUInt32(stream);
            }
        }

        private static void ReadStts(Stream stream, TrackInfo trackInfo)
        {
            stream.Seek(4, SeekOrigin.Current);

            var count = StreamHelper.ReadUInt32(stream);

            if (count > 1024)
            {
                count = 0;
            }

            trackInfo.FrameCount = new uint[count];
            trackInfo.Durations = new uint[count];

            for (var index = 0; index < count; index++)
            {
                trackInfo.FrameCount[index] = StreamHelper.ReadUInt32(stream);
                trackInfo.Durations[index] = StreamHelper.ReadUInt32(stream);
            }
        }

        private static void ReadChap(Stream stream, uint offset, TrackInfo trackInfo)
        {
            var count = (offset - 8) / 4;

            if (count <= 0 || count >= 1024)
            {
                return;
            }

            trackInfo.Chapters = new uint[count];

            for (var index = 0; index < count; index++)
            {
                trackInfo.Chapters[index] = StreamHelper.ReadUInt32(stream);
            }
        }
        <<

        /// <summary>
        /// 
        /// </summary>
        private sealed class TrackInfo
        {
            public uint Id
            {
                get;
                set;
            }

            public long[] Samples
            {
                get;
                set;
            }

            public uint[] Durations
            {
                get;
                set;
            }

            public uint[] FrameCount
            {
                get;
                set;
            }

            public uint[] Chapters
            {
                get;
                set;
            }

            public uint TimeUnitPerSecond
            {
                get;
                set;
            }

            public string[] Titles
            {
                get;
                set;
            }

            //public bool Empty => BoxType.Empty.Equals(Type);

            public TrackInfo()
            {
                //Type = BoxType.Empty;
                Samples = Array.Empty<long>();
                Durations = Array.Empty<uint>();
                FrameCount = Array.Empty<uint>();
                Chapters = Array.Empty<uint>();
                Titles = Array.Empty<string>();
            }

            public bool IsValid()
            {
                return true;
            }
        }*/
    }
}
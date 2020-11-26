using LibraProgramming.Media.Common;
using LibraProgramming.Media.QuickTime.Chunks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace LibraProgramming.Media.QuickTime.Visitors
{
    internal sealed class MediaTrackVisitor : QuickTimeMediaVisitor
    {
        private readonly QuickTimeMediaExtractor extractor;
        private readonly Stream stream;
        private readonly IList<MediaTrack> tracks;
        private readonly Stack<TrackInfo> trackInfos;
        private TrackInfo primaryTrack;
        
        private uint? timeScale;

        public MediaTrackVisitor(
            QuickTimeMediaExtractor extractor,
            Stream stream,
            IList<MediaTrack> tracks)
        {
            this.extractor = extractor;
            this.stream = stream;
            this.tracks = tracks;

            trackInfos = new Stack<TrackInfo>();
            primaryTrack = null;
        }

        public override void Visit(RootChunk chunk)
        {

            base.Visit(chunk);


        }

        public override void VisitMvhd(MvhdChunk chunk)
        {
            timeScale = chunk.TimeScale;

            base.VisitMvhd(chunk);
        }

        public override void VisitTrak(TrakChunk chunk)
        {
            var actual = new TrackInfo
            {
                TimeScale = timeScale.GetValueOrDefault(600)
            };

            trackInfos.Push(actual);
            
            Console.WriteLine("[TRAK] Begin");

            base.VisitTrak(chunk);

            var last = trackInfos.Pop();

            if (false == ReferenceEquals(last, actual))
            {
                throw new Exception();
            }

            if (0 < last.Chapters.Length)
            {
                primaryTrack = last;
                Console.WriteLine("[TRAK] //set primary track");
            }
            else
            {
                if (null != primaryTrack)
                {
                    var index = Array.FindIndex(primaryTrack.Chapters, chapter => chapter == last.TrackId);

                    if (-1 < index)
                    {
                        // TrackInfo primaryTrack 
                        // TrackInfo last
                        // TrakChunk chunk

                        Console.WriteLine("[TRAK] //handle description track");

                        foreach(var offset in last.Offsets)
                        {
                            var track = new QuickTimeMediaTrack(extractor);
                            var position = stream.Seek(offset, SeekOrigin.Begin);
                            var text = StreamHelper.ReadPascalString(stream);

                            track.SetTitle(text);
                            track.SetDuration(TimeSpan.FromSeconds(last.Duration / last.TimeScale));

                            tracks.Add(track);

                            //Console.WriteLine($"[TRAK] //track: '{text}'");
                        }
                    }
                }
            }

            Console.WriteLine("[TRAK] End");
            Console.WriteLine();
        }

        public override void VisitTkhd(TkhdChunk chunk)
        {
            var info = trackInfos.Peek();

            info.TrackId = chunk.TrackId;
            info.Duration = chunk.Duration;
            //info.Duration = (TimeSpan.FromSeconds(chunk.Duration / timeScale));

            Console.WriteLine($"[TKHD] //track id: {chunk.TrackId}");
            Console.WriteLine($"[TKHD] //duration: {chunk.Duration:d8}");
            Console.WriteLine();

            base.VisitTkhd(chunk);
        }

        public override void VisitTref(TrefChunk chunk)
        {
            var info = trackInfos.Peek();

            base.VisitTref(chunk);
        }

        public override void VisitChap(ChapChunk chunk)
        {
            var count = chunk.Scenes.Length;

            if (0 < chunk.Scenes.Length)
            {
                var info = trackInfos.Peek();
                info.Chapters = chunk.Scenes;
            }

            Console.WriteLine($"[CHAP] //scenes: {count}");
            Console.WriteLine(" -index-   scene");

            for (var index = 0; index < Math.Min(3, count); index++)
            {
                var scene = chunk.Scenes[index];
                Console.WriteLine($"[{index:d8}] {scene:d8}");
            }

            if (3 < count)
            {
                var scene = chunk.Scenes[count - 1];
                Console.WriteLine("...");
                Console.WriteLine($"[{(count - 1):d8}] {scene:d8}");
            }

            Console.WriteLine();

            base.VisitChap(chunk);
        }

        public override void VisitStts(SttsChunk chunk)
        {
            Console.WriteLine($"[STTS] //descriptions: {chunk.FrameDescriptions.Length}");

            var count = chunk.FrameDescriptions.Length;

            Console.WriteLine(" -index-   frames   duration");

            for (var index = 0; index < Math.Min(3, count); index++)
            {
                var description = chunk.FrameDescriptions[index];
                Console.WriteLine($"[{index:d8}] {description.FrameCount:d8} {description.Duration:d8}");
            }

            if (3 < count)
            {
                var description = chunk.FrameDescriptions[count - 1];
                Console.WriteLine("...");
                Console.WriteLine($"[{(count - 1):d8}] {description.FrameCount:d8} {description.Duration:d8}");
            }

            Console.WriteLine();

            base.VisitStts(chunk);
        }

        public override void VisitStsz(StszChunk chunk)
        {
            var sampleSize = chunk.SampleSize;

            if (0 < sampleSize)
            {
                Console.WriteLine($"[STSZ] //common sample size: {sampleSize})");
                return;
            }

            var sampleSizes = chunk.SampleSizes.Length;

            Console.WriteLine($"[STSZ] sample sizes: {sampleSizes}");
            Console.WriteLine(" -index-   samp/sizes");

            for (var index = 0; index < Math.Min(3, sampleSizes); index++)
            {
                var size = chunk.SampleSizes[index];
                Console.WriteLine($"[{index:d8}] {size:d8}");
            }

            if (3 < sampleSizes)
            {
                var size = chunk.SampleSizes[sampleSizes - 1];
                Console.WriteLine("...");
                Console.WriteLine($"[{(sampleSizes - 1):d8}] {size:d8}");
            }

            Console.WriteLine();

            base.VisitStsz(chunk);
        }

        public override void VisitStco(StcoChunk chunk)
        {
            var info = trackInfos.Peek();

            info.Offsets = chunk.Offsets;

            var count = chunk.Offsets.Length;

            Console.WriteLine($"[STCO] //offsets: {count}");
            Console.WriteLine(" -index-   offsets");

            for (var index = 0; index < Math.Min(3, count); index++)
            {
                var offset = chunk.Offsets[index];
                Console.WriteLine($"[{index:d8}] {offset:d8}");
            }

            if (3 < count)
            {
                var offset = chunk.Offsets[count - 1];

                Console.WriteLine("...");
                Console.WriteLine($"[{(count - 1):d8}] {offset:d8}");
            }

            Console.WriteLine();

            base.VisitStco(chunk);
        }

        public override void VisitStsc(StscChunk chunk)
        {
            var count = chunk.BlockDescriptions.Length;

            Console.WriteLine($"[STSC] blocks: {count}");
            Console.WriteLine(" -index-   f/chnk   samples  id");

            for (var index = 0; index < Math.Min(3, count); index++)
            {
                var block = chunk.BlockDescriptions[index];
                Console.WriteLine($"[{index:d8}] {block.FirstChunk:d8} {block.SamplesPerChunk:d8} {block.SampleDurationIndex:d6}");
            }

            if (3 < count)
            {
                var block = chunk.BlockDescriptions[count - 1];
                Console.WriteLine("...");
                Console.WriteLine($"[{(count - 1):d8}] {block.FirstChunk:d8} {block.SamplesPerChunk:d8} {block.SampleDurationIndex:d6}");
            }

            Console.WriteLine();

            base.VisitStsc(chunk);
        }

        private sealed class TrackInfo
        {
            public uint TrackId
            {
                get;
                set;
            }

            public uint TimeScale
            {
                get;
                set;
            }

            public ulong Duration
            {
                get;
                set;
            }

            public uint[] Chapters
            {
                get;
                set;
            }

            public uint[] Offsets
            {
                get;
                set;
            }

            public TrackInfo()
            {
                Chapters = Array.Empty<uint>();
                Offsets = Array.Empty<uint>();
            }
        }
    }
}

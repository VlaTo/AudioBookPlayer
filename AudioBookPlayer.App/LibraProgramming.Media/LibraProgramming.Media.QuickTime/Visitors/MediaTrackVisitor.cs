using LibraProgramming.Media.Common;
using LibraProgramming.Media.QuickTime.Chunks;
using System;
using System.Collections.Generic;
using System.IO;

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
            Console.WriteLine();

            base.Visit(chunk);

            ;
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
            }
            else
            {
                if (null != primaryTrack)
                {
                    var chapterIndex = Array.FindIndex(primaryTrack.Chapters, chapter => chapter == last.TrackId);

                    if (-1 < chapterIndex)
                    {
                        // TrackInfo primaryTrack 
                        // TrackInfo last
                        // TrakChunk chunk

                        //Console.WriteLine($"[TRAK] //primary track time scale: {primaryTrack.TimeScale}");
                        //Console.WriteLine($"[TRAK] //last track time scale: {last.TimeScale}");

                        var samplesCount = 0u;
                        var samplesDuration = 0u;

                        for (var index = 0; index < last.Offsets.Length; index++)
                        {
                            var track = new QuickTimeMediaTrack(extractor);
                            var offset = last.Offsets[index];
                            var timeToSample = last.Entries[index];
                            var position = stream.Seek(offset, SeekOrigin.Begin);
                            var text = StreamHelper.ReadPascalString(stream);

                            track.SetTitle(text);
                            track.SetDuration(TimeSpan.FromSeconds(timeToSample.Duration / last.TimeScale));

                            tracks.Add(track);

                            samplesCount += timeToSample.SampleCount;
                            samplesDuration += timeToSample.Duration;
                        }

                        Console.WriteLine($"{TimeSpan.FromSeconds(samplesDuration / last.TimeScale)}");
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
            Console.WriteLine($"[TKHD] //created: {chunk.Created}");
            Console.WriteLine($"[TKHD] //modified: {chunk.Modified}");
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
            var info = trackInfos.Peek();

            info.Entries = chunk.Entries;

            Console.WriteLine($"[STTS] //entries: {chunk.Entries.Length}");

            var count = chunk.Entries.Length;

            Console.WriteLine(" -index-   samples   duration");

            for (var index = 0; index < Math.Min(3, count); index++)
            {
                var description = chunk.Entries[index];
                Console.WriteLine($"[{index:d8}] {description.SampleCount:d8} {description.Duration:d8}");
            }

            if (3 < count)
            {
                var description = chunk.Entries[count - 1];
                Console.WriteLine("...");
                Console.WriteLine($"[{(count - 1):d8}] {description.SampleCount:d8} {description.Duration:d8}");
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

            Console.WriteLine($"[STSZ] //sample sizes: {sampleSizes}");
            //Console.WriteLine(" -index-   samp/sizes");

            for (var index = 0; index < Math.Min(3, sampleSizes); index++)
            {
                var size = chunk.SampleSizes[index];
                Console.WriteLine($"[{index:d8}] {size:d8} byte(s)");
            }

            if (3 < sampleSizes)
            {
                var size = chunk.SampleSizes[sampleSizes - 1];
                Console.WriteLine("...");
                Console.WriteLine($"[{(sampleSizes - 1):d8}] {size:d8} byte(s)");
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

            Console.WriteLine($"[STSC] //blocks: {count}");
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

        /// <summary>
        /// 
        /// </summary>
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

            public TimeToSample[] Entries
            {
                get;
                set;
            }

            public TrackInfo()
            {
                Chapters = Array.Empty<uint>();
                Offsets = Array.Empty<uint>();
                Entries = Array.Empty<TimeToSample>();
            }
        }
    }
}

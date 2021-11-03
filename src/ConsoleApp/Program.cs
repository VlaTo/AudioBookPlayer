using LibraProgramming.Media.Common;
using LibraProgramming.Media.QuickTime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var source = args[0];

            using (var stream = File.OpenRead(source))
            {
                IReadOnlyCollection<IMediaTrack> tracks = null;

                using (var extractor = QuickTimeMediaExtractor.CreateFrom(stream))
                {
                    extractor.Debug();

                    var items = extractor.GetMediaTags();

                    tracks = extractor.GetTracks();

                    Console.WriteLine();
                    Console.WriteLine(" *** Info ***");

                    foreach (var (key, meta) in items)
                    {
                        Console.WriteLine($"[Meta] {key}");

                        foreach (var item in meta)
                        {
                            if (item is TextValue textItem)
                            {
                                Console.WriteLine($"[Meta]    Text: '{textItem.Text}'");
                            }
                            else if (item is StreamValue streamItem)
                            {
                                Console.WriteLine($"[Meta]    Stream: {streamItem.Stream.Length} bytes");
                            }
                            else if (item is BinaryValue binaryItem)
                            {
                                Console.WriteLine($"[Meta]    Binary: {binaryItem.Bytes.Length} bytes");
                            }
                        }
                    }

                    Console.WriteLine();
                }

                var total = TimeSpan.Zero;

                foreach (var track in tracks)
                {
                    Console.WriteLine($"[Track] '{track.Title}' {track.Duration:hh':'mm':'ss}");
                    total += track.Duration;
                }

                Console.WriteLine();
                Console.WriteLine($"[TOTAL] length: {total:hh':'mm':'ss}");

                /*var audioTrack = tracks.First();

                using (var audio = audioTrack.GetMediaStream())
                {
                    using (var target = File.Create(output))
                    {
                        audio.CopyTo(target);
                    }
                }*/
            }
        }
    }
}

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
            var output = args[1];

            using (var stream = File.OpenRead(source))
            {
                IReadOnlyCollection<IMediaTrack> tracks = null;

                using (var extractor = QuickTimeMediaExtractor.CreateFrom(stream))
                {
                    extractor.Debug();

                    var meta = extractor.GetMeta();

                    tracks = extractor.GetTracks();

                    Console.WriteLine();
                    Console.WriteLine(" *** Info ***");

                    foreach (var item in meta.Items)
                    {
                        switch (item)
                        {
                            case MetaInformationTextItem textItem:
                            {
                                Console.WriteLine($"[Meta] {textItem.Key} = '{textItem.Text}'");

                                if (WellKnownMetaItemNames.Cover.Equals(item.Key))
                                {
                                }

                                break;
                            }

                            case MetaInformationStreamItem streamItem:
                            {
                                Console.WriteLine($"[Meta] {streamItem.Key} = binary {streamItem.Stream.Length:N} byte(s)");
                                break;
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

                var audioTrack = tracks.First();

                using (var audio = audioTrack.GetMediaStream())
                {
                    using (var target = File.Create(output))
                    {
                        audio.CopyTo(target);
                    }
                }
            }
        }
    }
}

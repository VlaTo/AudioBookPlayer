﻿using LibraProgramming.Media.Common;
using LibraProgramming.Media.QuickTime;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var filename = args[0];

            using (var extractor = await QuickTimeMediaExtractor.CreateFromAsync(File.OpenRead(filename)))
            {
                extractor.Debug();

                var meta = extractor.GetMeta();
                var tracks = extractor.GetTracks();

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

                var total = TimeSpan.Zero;

                foreach (var track in tracks)
                {
                    Console.WriteLine($"[Track] '{track.Title}' {track.Duration:hh':'mm':'ss}");
                    total += track.Duration;
                }

                Console.WriteLine();

                Console.WriteLine($"[TOTAL] length: {total:c}");
            }
        }
    }
}

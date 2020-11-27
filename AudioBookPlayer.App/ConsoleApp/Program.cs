using LibraProgramming.Media.Common;
using LibraProgramming.Media.QuickTime;
using System;
using System.IO;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var filename = args[0];

            using (var extractor = QuickTimeMediaExtractor.CreateFrom(File.OpenRead(filename)))
            {
                extractor.Debug();

                // Metadata
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

                foreach(var track in tracks)
                {
                    Console.WriteLine($"[Track] '{track.Title}' {track.Duration:G}");
                }
            }
        }
    }
}

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

                Console.WriteLine();

                foreach (var item in meta.Items)
                {
                    switch (item)
                    {
                        case MetaInformationTextItem textItem:
                        {
                            Console.WriteLine($"[META] key: {textItem.Key} = '{textItem.Text}'");

                            if (WellKnownMetaItemNames.Cover.Equals(item.Key))
                            {
                            }

                            break;
                        }

                        case MetaInformationStreamItem streamItem:
                        {
                            Console.WriteLine($"[META] key: {streamItem.Key}:{streamItem.Stream.Length}");
                            break;
                        }
                    }
                }

                Console.WriteLine();

                var tracks = extractor.GetTracks();

                foreach(var track in tracks)
                {
                    Console.WriteLine($"[Track] '{track.Title}' {track.Duration:G}");
                }


                //var cover = meta.Items.Find(item => item.Key == "");

                //var tracksCount = extractor.GetTracksCount();
                //var buffer = new byte[4096];

                //for (var index = 0; index < tracksCount; index++)
                //{
                    //var track = extractor.GetTrack(index);
                    //var sampleSize = track.ReadSampleData(buffer);
                //}
            }

            //System.Console.ReadLine();
        }
    }
}

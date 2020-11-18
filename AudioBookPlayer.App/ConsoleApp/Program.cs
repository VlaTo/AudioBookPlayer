using LibraProgramming.Media.Common;
using LibraProgramming.Media.QuickTime;
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

                var meta = extractor.GetMeta();

                foreach (var item in meta.Items)
                {
                    if (item.Key == "Cover")
                    {
                        const string outputFilename = "d:/Temp/cover.jpg";
                        var source = (MetaInformationStreamItem)item;
                        using(var target = File.OpenWrite(outputFilename))
                        {
                            source.Stream.CopyTo(target);
                            System.Console.WriteLine($"File: '{outputFilename}' saved.");
                        }
                    }
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

            System.Console.ReadLine();
        }
    }
}

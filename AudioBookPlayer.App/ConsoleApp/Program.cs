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

                var tracks = extractor.GetTracks();

                foreach(var track in tracks)
                {
                    ;
                }

                /*var meta = extractor.GetMeta();

                foreach (var item in meta.Items)
                {
                    if (WellKnownMetaItemNames.Cover.Equals(item.Key))
                    {
                        var source = (MetaInformationStreamItem)item;
                    }
                }*/

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

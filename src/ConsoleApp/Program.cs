using LibraProgramming.Media.QuickTime;

var filename = args[0];

using (var extractor = QuickTimeMediaExtractor.CreateFrom(File.OpenRead(filename)))
{
    extractor.Debug();

    var tags = extractor.GetMediaTags();

    foreach (var kvp in tags)
    {
        /*if (kvp.Key == "Cover")
        {
            const string outputFilename = "d:/Temp/cover.jpg";
            var source = (MetaInformationStreamItem)kvp;
            using (var target = File.OpenWrite(outputFilename))
            {
                source.Stream.CopyTo(target);
                System.Console.WriteLine($"File: '{outputFilename}' saved.");
            }
        }*/
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

Console.ReadLine();
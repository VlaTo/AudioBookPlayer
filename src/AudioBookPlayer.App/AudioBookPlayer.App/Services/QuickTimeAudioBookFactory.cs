using AudioBookPlayer.App.Core;
using LibraProgramming.Media.Common;
using LibraProgramming.Media.QuickTime;
using System;
using System.Collections.Generic;
using System.IO;

namespace AudioBookPlayer.App.Services
{
    internal sealed class QuickTimeAudioBookFactory : IAudioBookFactory
    {
        private MediaInformation extractMediaInfo;

        public QuickTimeAudioBookFactory()
        {
        }

        public MediaInformation ExtractMediaInfo(Stream stream)
        {
            using (StopwatchMeter.Start("ExtractMediaInfo inner"))
            {
                return Temp(stream);
            }
        }

        private MediaInformation Temp(Stream stream)
        {
            MetaInformation originalMeta;
            IReadOnlyCollection<IMediaTrack> originalTracks;

            using (var extractor = QuickTimeMediaExtractor.CreateFrom(stream))
            {
                using (StopwatchMeter.Start("#1 GetMeta"))
                {
                    originalMeta = extractor.GetMeta();
                }

                using (StopwatchMeter.Start("#2 GetTracks"))
                {
                    originalTracks = extractor.GetTracks();
                }

                //var originalMeta = extractor.GetMeta();
                //var originalTracks = extractor.GetTracks();

                //var meta = new List<MetaItemValue>();
                string bookTitle = null;
                List<string> bookAuthors = new List<string>();
                ushort? bookYear;

                using (StopwatchMeter.Start("#3 Foreach meta"))
                {
                    foreach (var (key, meta) in originalMeta)
                    {
                        switch (key)
                        {
                            case WellKnownMetaItemNames.Title:
                            {

                                foreach (var item in meta)
                                {
                                    if (item is TextItemValue textMeta)
                                    {
                                        ;
                                    }
                                }

                                break;
                            }

                            case WellKnownMetaItemNames.Author:
                            {
                                foreach (var item in meta)
                                {
                                    if (item is TextItemValue textMeta)
                                    {
                                        if (bookAuthors.Contains(textMeta.Text))
                                        {
                                            continue;
                                        }

                                        bookAuthors.Add(textMeta.Text);
                                    }
                                }

                                break;
                            }

                            /*case StreamItemValue streamItem:
                            {
                                if (WellKnownMetaItemNames.Cover.Equals(streamItem.Key))
                                {
                                    var bookImage = new InMemoryAudioBookImage(audioBook, streamItem.Key,
                                        streamItem.Stream.ToBytes());
                                    audioBook.Images.Add(bookImage);
                                }
                                else
                                {
                                    Debug.WriteLine($"[Binary Meta] key: '{streamItem.Key}'");
                                }

                                break;
                            }*/
                        }
                    }
                }

                var tracks = new List<IMediaTrack>();
                var start = TimeSpan.Zero;

                using (StopwatchMeter.Start("#4 Add AudioTracks"))
                {
                    foreach (var track in originalTracks)
                    {

                        /*var chapter = new AudioBookChapter(audioBook, track.Title, start);
                        var fragment = new AudioBookChapterFragment(start, track.Duration, null);
    
                        audioBook.Chapters.Add(chapter);
                        audioBook.SourceFiles.Add(new AudioBookSourceFile(audioBook, file));
    
                        chapter.Fragments.Add(fragment);*/

                        tracks.Add(track);

                        start += track.Duration;
                    }
                }

                using (StopwatchMeter.Start("#5 Create MediaInformation"))
                {
                    extractMediaInfo = new MediaInformation(tracks, originalMeta, bookTitle, bookAuthors.ToArray(), start);
                }
            }

            return extractMediaInfo;
        }
    }
}
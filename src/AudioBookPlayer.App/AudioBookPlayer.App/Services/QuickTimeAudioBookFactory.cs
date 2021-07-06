using AudioBookPlayer.App.Models;
using LibraProgramming.Media.Common;
using LibraProgramming.Media.QuickTime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace AudioBookPlayer.App.Services
{
    internal sealed class QuickTimeAudioBookFactory : IAudioBookFactory
    {
        public QuickTimeAudioBookFactory()
        {
        }

        public MediaInformation ExtractMediaInfo(Stream stream)
        {
            using (var extractor = QuickTimeMediaExtractor.CreateFrom(stream))
            {
                var originalMeta = extractor.GetMeta();
                var originalTracks = extractor.GetTracks();
                
                var meta = new List<MetaInformationItem>();
                string bookTitle = null;
                List<string> bookAuthors = new List<string>();
                ushort? bookYear;

                foreach (var item in originalMeta)
                {
                    switch (item)   
                    {
                        case MetaInformationTextItem textItem:
                        {
                            if (WellKnownMetaItemNames.Title.Equals(textItem.Key))
                            {
                                bookTitle = textItem.Text;
                            }
                            else if (WellKnownMetaItemNames.Author.Equals(textItem.Key))
                            {
                                var index = bookAuthors.FindIndex(author => String.Equals(author, textItem.Text));

                                if (0 > index)
                                {
                                    bookAuthors.Add(textItem.Text);
                                }
                            }
                            else
                            {
                                meta.Add(item);
                            }

                            break;
                        }

                        case MetaInformationStreamItem streamItem:
                        {
                            if (WellKnownMetaItemNames.Cover.Equals(streamItem.Key))
                            {
                                /*var bookImage = new InMemoryAudioBookImage(audioBook, streamItem.Key,
                                    streamItem.Stream.ToBytes());
                                audioBook.Images.Add(bookImage);*/
                            }
                            else
                            {
                                Debug.WriteLine($"[Binary Meta] key: '{streamItem.Key}'");
                            }

                            break;
                        }
                    }
                }

                var tracks = new List<IMediaTrack>();
                var start = TimeSpan.Zero;

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

                return new MediaInformation(tracks, meta, bookTitle, bookAuthors.ToArray(), start);
            }
        }
    }
}
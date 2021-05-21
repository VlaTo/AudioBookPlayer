using System;
using System.IO;
using AudioBookPlayer.App.Core.Extensions;
using AudioBookPlayer.App.Models;
using LibraProgramming.Media.Common;
using LibraProgramming.Media.QuickTime;

namespace AudioBookPlayer.App.Services
{
    internal sealed class QuickTimeAudioBookFactory : IAudioBookFactory
    {
        public QuickTimeAudioBookFactory()
        {
        }

        public AudioBook CreateAudioBook(string folder, string filename, int level)
        {
            var path = Path.Combine(folder, filename);
            return ExtractMediaInfo(path);
        }

        private static AudioBook ExtractMediaInfo(string file)
        {
            using (var stream = File.Open(file, FileMode.Open, FileAccess.Read))
            {
                using (var extractor = QuickTimeMediaExtractor.CreateFrom(stream))
                {
                    var audioBook = new AudioBook();
                    var meta = extractor.GetMeta();
                    var tracks = extractor.GetTracks();

                    foreach (var item in meta.Items)
                    {
                        switch (item)
                        {
                            case MetaInformationTextItem textItem:
                            {
                                if (WellKnownMetaItemNames.Title.Equals(textItem.Key))
                                {
                                    audioBook.Title = textItem.Text;
                                }
                                else
                                if (WellKnownMetaItemNames.Author.Equals(textItem.Key))
                                {
                                    audioBook.Authors.Add(textItem.Text);
                                }
                                else
                                if (WellKnownMetaItemNames.Subtitle.Equals(textItem.Key))
                                {
                                    audioBook.Synopsis = textItem.Text;
                                }

                                break;
                            }

                            case MetaInformationStreamItem streamItem:
                            {
                                if (WellKnownMetaItemNames.Cover.Equals(streamItem.Key))
                                {
                                    var bookImage = new InMemoryAudioBookImage(streamItem.Key, streamItem.Stream.ToBytes());
                                    audioBook.Images.Add(bookImage);
                                }

                                break;
                            }
                        }
                    }

                    var duration = TimeSpan.Zero;

                    foreach (var track in tracks)
                    {
                        var chapter = new AudioBookChapter
                        {
                            Title = track.Title,
                            Start = duration,
                            Duration = track.Duration,
                            SourceFile = file
                        };

                        audioBook.Chapters.Add(chapter);
                        //Console.WriteLine($"[Track] '{track.Title}' {track.Duration:hh':'mm':'ss}");

                        duration += track.Duration;
                    }

                    audioBook.Duration = duration;

                    return audioBook;
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using AudioBookPlayer.Domain.Models;

namespace AudioBookPlayer.MediaBrowserService.Core
{
    internal sealed class AudioBookBuilder
    {
        private readonly List<string> authors;
        private readonly List<string> description;
        private readonly List<AudioBookSectionBuilder> sections;
        private readonly List<ReadOnlyMemory<byte>> images;
        
        public string Title
        {
            get;
            private set;
        }

        public long MediaId
        {
            get;
            private set;
        }

        private AudioBookBuilder()
        {
            authors = new List<string>();
            description = new List<string>();
            sections = new List<AudioBookSectionBuilder>();
            images = new List<ReadOnlyMemory<byte>>();
        }

        public static AudioBookBuilder Create()
        {
            return new AudioBookBuilder();
        }

        public AudioBook Build()
        {
            var audioBook = new AudioBook
            {
                Title = Title,
                MediaId = MediaId,
                Created = DateTime.UtcNow
            };

            audioBook.Authors = BuildAuthors(audioBook);
            audioBook.Sections = BuildSections(audioBook);
            audioBook.Chapters = BuildChapters(audioBook);
            audioBook.Images = BuildImages(audioBook);

            FixChapterOffsets(audioBook);
            FixHash(audioBook);

            return audioBook;
        }

        public AudioBookBuilder SetTitle(string value)
        {
            Title = value;
            return this;
        }

        public AudioBookBuilder SetMediaId(long value)
        {
            MediaId = value;
            return this;
        }

        public AudioBookBuilder SetAuthors(IEnumerable<string> value)
        {
            authors.Clear();
            return AddAuthors(value);
        }

        public AudioBookBuilder AddAuthors(IEnumerable<string> value)
        {
            authors.AddRange(value);
            return this;
        }

        public AudioBookBuilder AddAuthor(string value)
        {
            authors.Add(value);
            return this;
        }

        public AudioBookBuilder SetDescription(string value)
        {
            description.Clear();
            return AddDescription(value);
        }

        public AudioBookBuilder AddDescription(string value)
        {
            description.Add(value);
            return this;
        }

        public AudioBookBuilder AddDescriptions(IEnumerable<string> value)
        {
            description.AddRange(value);
            return this;
        }

        public AudioBookSectionBuilder NewSection()
        {
            var builder = new AudioBookSectionBuilder(this);
            
            sections.Add(builder);

            return builder;
        }

        public AudioBookBuilder AddImage(ReadOnlyMemory<byte> value)
        {
            images.Add(value);
            return this;
        }

        private IReadOnlyList<AudioBookAuthor> BuildAuthors(AudioBook audioBook)
        {
            var list = new AudioBookAuthor[authors.Count];

            for (var authorIndex = 0; authorIndex < authors.Count; authorIndex++)
            {
                var name = authors[authorIndex];
                list[authorIndex] = new AudioBookAuthor(audioBook, name);
            }

            return list;
        }

        private IReadOnlyList<AudioBookSection> BuildSections(AudioBook audioBook)
        {
            var list = new AudioBookSection[sections.Count];

            for (var sectionIndex = 0; sectionIndex < sections.Count; sectionIndex++)
            {
                var builder = sections[sectionIndex];
                list[sectionIndex] = builder.Build(audioBook);
            }

            return list;
        }

        private IReadOnlyList<AudioBookChapter> BuildChapters(AudioBook audioBook)
        {
            var list = new List<AudioBookChapter>();

            for (var sectionIndex = 0; sectionIndex < audioBook.Sections.Count; sectionIndex++)
            {
                var section = audioBook.Sections[sectionIndex];
                list.AddRange(section.Chapters);
            }

            return list;
        }

        private IReadOnlyList<IAudioBookImage> BuildImages(AudioBook audioBook)
        {
            var list = new List<IAudioBookImage>();

            for (var imageIndex = 0; imageIndex < images.Count; imageIndex++)
            {
                var image = new InMemoryImage(audioBook, images[imageIndex]);
                list.Add(image);
            }

            return list;
        }

        private static void FixChapterOffsets(AudioBook audioBook)
        {
            var bookDuration = TimeSpan.Zero;

            for (var chapterIndex = 0; chapterIndex < audioBook.Chapters.Count; chapterIndex++)
            {
                var chapter = audioBook.Chapters[chapterIndex];

                chapter.Offset = bookDuration;
                bookDuration += chapter.Duration;
            }

            audioBook.Duration = bookDuration;
        }

        private void FixHash(AudioBook audioBook)
        {
            audioBook.Hash = GetHashCode();
        }
    }

    internal sealed class AudioBookSectionBuilder
    {
        private readonly AudioBookBuilder bookBuilder;
        private readonly List<AudioBookChapterBuilder> chapters;
        private string title;
        private string sourceFileUri;

        public AudioBookSectionBuilder(AudioBookBuilder bookBuilder)
        {
            this.bookBuilder = bookBuilder;
            chapters = new List<AudioBookChapterBuilder>();
        }

        public AudioBookSection Build(AudioBook audioBook)
        {
            var section = new AudioBookSection(audioBook)
            {
                Title = title,
                SourceFileUri = sourceFileUri
            };

            section.Chapters = BuildChapters(audioBook, section);

            return section;
        }

        public AudioBookSectionBuilder SetTitle(string value)
        {
            title = value;
            return this;
        }

        public AudioBookSectionBuilder SetSourceFileUri(string value)
        {
            sourceFileUri = value;
            return this;
        }

        public AudioBookChapterBuilder NewChapter()
        {
            var builder = new AudioBookChapterBuilder();

            chapters.Add(builder);

            return builder;
        }

        private IReadOnlyList<AudioBookChapter> BuildChapters(AudioBook audioBook, AudioBookSection section)
        {
            var list = new AudioBookChapter[chapters.Count];

            for (var chapterIndex = 0; chapterIndex < chapters.Count; chapterIndex++)
            {
                var chapter = chapters[chapterIndex].Build(audioBook, section);
                list[chapterIndex] = chapter;
            }

            return list;
        }
    }

    internal sealed class AudioBookChapterBuilder
    {
        private string title;
        private TimeSpan duration;
        
        public AudioBookChapterBuilder()
        {
            title = String.Empty;
            duration = TimeSpan.Zero;
        }

        public AudioBookChapter Build(AudioBook audioBook, AudioBookSection audioBookSection)
        {
            var chapter = new AudioBookChapter(audioBook, audioBookSection)
            {
                Title = title,
                Duration = duration
            };

            return chapter;
        }

        public AudioBookChapterBuilder SetTitle(string value)
        {
            title = value;
            return this;
        }

        public AudioBookChapterBuilder SetDuration(TimeSpan value)
        {
            duration = value;
            return this;
        }
    }
}
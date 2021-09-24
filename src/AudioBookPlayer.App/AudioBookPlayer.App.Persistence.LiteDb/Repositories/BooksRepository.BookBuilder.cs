namespace AudioBookPlayer.App.Persistence.LiteDb.Repositories
{
    public partial class BooksRepository
    {
        /// <summary>
        /// Maps business-object <see cref="AudioBook" /> to DTO-object <see cref="Book" />.
        /// </summary>
        private sealed class BookBuilder
        {
            /*private readonly ICoverService coverService;

            private BookBuilder(ICoverService coverService)
            {
                this.coverService = coverService;
            }

            public static BookBuilder Create(ICoverService coverService)
            {
                var instance = new BookBuilder(coverService);
                return instance;
            }

            public async Task<Book> MapFromAsync(AudioBook entity)
            {
                var book = new Book
                {
                    Title = entity.Title,
                    Synopsis = entity.Synopsis,
                };

                FixDateCreated(book, entity.Created);
                AddAuthors(book, entity.Authors);
                AddSections(book, entity.Sections);

                await AddCoversAsync(book, entity.Images, CancellationToken.None);

                return book;
            }

            private static void FixDateCreated(Book book, DateTime? source)
            {
                book.Created = source.GetValueOrDefault(DateTime.UtcNow);
            }

            private static void AddAuthors(Book book, IList<AudioBookAuthor> source)
            {
                var authors = new string[source.Count];

                for (var index = 0; index < source.Count; index++)
                {
                    authors[index] = source[index].Name;
                }

                book.Authors = authors;
            }

            private async Task AddCoversAsync(Book book, IList<AudioBookImage> images, CancellationToken cancellationToken)
            {
                var covers = new List<string>();

                for (var index = 0; index < images.Count; index++)
                {
                    var source = images[index];

                    if (source is IHasContentUri cover)
                    {
                        covers.Add(cover.ContentUri);
                        continue;
                    }

                    await using (var stream = await source.GetStreamAsync(cancellationToken))
                    {
                        var contentUri = await coverService.AddImageAsync(stream, cancellationToken);
                        covers.Add(contentUri);
                    }
                }

                book.Images = covers.ToArray();
            }

            private static void AddSections(Book book, IList<AudioBookSection> audioBookSections)
            {
                var parts = new Section[audioBookSections.Count];

                for (var index = 0; index < audioBookSections.Count; index++)
                {
                    var audioBookPart = audioBookSections[index];
                    var part = new Section
                    {
                        Title = audioBookPart.Title,

                    };

                    parts[index] = part;

                    AddChapters(part, audioBookPart);

                }

                book.Sections = parts;
            }

            private static void AddChapters(Section section, AudioBookSection audioBookSection)
            {
                var chapters = new Chapter[audioBookSection.Chapters.Count];

                for (var index = 0; index < audioBookSection.Chapters.Count; index++)
                {
                    var audioBookChapter = audioBookSection.Chapters[index];
                    var chapter = new Chapter
                    {
                        Title = audioBookChapter.Title,
                        Start = audioBookChapter.Start
                    };

                    chapters[index] = chapter;

                    AddFragments(chapter, audioBookChapter);
                }

                section.Chapters = chapters;
            }

            private static void AddFragments(Chapter chapter, AudioBookChapter audioBookChapter)
            {
                var fragments = new Fragment[audioBookChapter.Fragments.Count];

                for (var index = 0; index < audioBookChapter.Fragments.Count; index++)
                {
                    var audioBookFragment = audioBookChapter.Fragments[index];
                    var fragment = new Fragment
                    {
                        Start = audioBookFragment.Start,
                        Duration = audioBookFragment.Duration,
                        ContentUri = audioBookFragment.SourceFile.ContentUri
                    };

                    fragments[index] = fragment;
                }

                chapter.Fragments = fragments;
            }*/
        }
    }
}
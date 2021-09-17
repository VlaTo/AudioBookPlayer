using AudioBookPlayer.App.Domain.Models;
using AudioBookPlayer.App.Domain.Services;
using AudioBookPlayer.App.Persistence.LiteDb.Models;
using System;
using System.Collections.Generic;

namespace AudioBookPlayer.App.Persistence.LiteDb.Repositories
{
    /*public partial class BooksRepository
    {
        /// <summary>
        /// Maps DTO object <see cref="Book" /> to the business-object <see cref="AudioBook" />.
        /// </summary>
        private sealed class AudioBookBuilder
        {
            private readonly ICoverService coverService;

            private AudioBookBuilder(ICoverService coverService)
            {
                this.coverService = coverService;
            }

            public static AudioBookBuilder Create(ICoverService coverService)
            {
                var builder = new AudioBookBuilder(coverService);
                return builder;
            }

            public AudioBook MapFrom(Book source)
            {
                var entity = new AudioBook(source.Title, source.Id)
                {
                    Synopsis = source.Synopsis
                };

                FixAddedToLibrary(entity, source.Created);
                AddAuthors(entity.Authors, source.Authors);
                AddSections(entity, source.Sections);
                AddImages(entity, source.Images);

                return entity;
            }
            
            private static void FixAddedToLibrary(AudioBook audioBook, DateTime source)
            {
                audioBook.Created = source.ToLocalTime();
            }
            
            private static void AddAuthors(IList<AudioBookAuthor> authors, string[] source)
            {
                for (var index = 0; index < source.Length; index++)
                {
                    var author = new AudioBookAuthor(source[index]);
                    authors.Add(author);
                }
            }

            private static void AddSections(AudioBook audioBook, Section[] sourceSections)
            {
                var sections = new Dictionary<string, AudioBookSection>();

                AudioBookSection GetOrCreateSection(string title)
                {
                    if (sections.TryGetValue(title, out var section))
                    {
                        return section;
                    }

                    section = new AudioBookSection(audioBook, title);

                    sections.Add(title, section);
                    audioBook.Sections.Add(section);

                    return section;
                }

                for (var index = 0; index < sourceSections.Length; index++)
                {
                    var sourceSection = sourceSections[index];
                    var audioBookSection = GetOrCreateSection(sourceSection.Title);

                    AddChapters(audioBookSection, sourceSection.Chapters);
                }
            }
            
            private void AddImages(AudioBook audioBook, string[] source)
            {
                for (var index = 0; index < source.Length; index++)
                {
                    audioBook.Images.Add(
                        new ContentProvidedAudioBookImage(audioBook, source[index], coverService)
                    );
                }
            }
            
            private static void AddChapters(AudioBookSection section, Chapter[] sourceChapters)
            {
                var audioBook = section.AudioBook;

                for (var index = 0; index < sourceChapters.Length; index++)
                {
                    var sourceChapter = sourceChapters[index];
                    var chapter = new AudioBookChapter(section.AudioBook, sourceChapter.Title, sourceChapter.Start, section);

                    section.Chapters.Add(chapter);
                    audioBook.Chapters.Add(chapter);

                    AddFragments(chapter, sourceChapter.Fragments);
                }
            }

            private static void AddFragments(AudioBookChapter chapter, Fragment[] sourceFragments)
            {
                var audioBook = chapter.AudioBook;
                var sourceFiles = new Dictionary<string, AudioBookSourceFile>();

                AudioBookSourceFile FindSourceFile(string contentUri)
                {
                    for (var index = 0; index < audioBook.SourceFiles.Count; index++)
                    {
                        var sourceFile = audioBook.SourceFiles[index];
                        var found = String.Equals(
                            sourceFile.ContentUri,
                            contentUri,
                            StringComparison.InvariantCulture
                        );

                        if (found)
                        {
                            return sourceFile;
                        }
                    }

                    return null;
                }

                AudioBookSourceFile GetOrCreate(string contentUri)
                {
                    if (false == sourceFiles.TryGetValue(contentUri, out var sourceFile))
                    {
                        sourceFile = FindSourceFile(contentUri);

                        if (null == sourceFile)
                        {
                            sourceFile = new AudioBookSourceFile(audioBook, contentUri);
                            audioBook.SourceFiles.Add(sourceFile);
                        }

                        sourceFiles.Add(contentUri, sourceFile);
                    }

                    return sourceFile;
                }

                for (var index = 0; index < sourceFragments.Length; index++)
                {
                    var sourceFragment = sourceFragments[index];
                    var source = GetOrCreate(sourceFragment.ContentUri);
                    var fragment = new AudioBookChapterFragment(sourceFragment.Start, sourceFragment.Duration, source);

                    chapter.Fragments.Add(fragment);
                }
            }
        }
    }*/
}
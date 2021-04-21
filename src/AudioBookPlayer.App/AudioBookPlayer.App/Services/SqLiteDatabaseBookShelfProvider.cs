﻿using AudioBookPlayer.App.Data;
using AudioBookPlayer.App.Models;
using LibraProgramming.Xamarin.Dependency.Container.Attributes;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace AudioBookPlayer.App.Services
{
    internal sealed class SqLiteDatabaseBookShelfProvider : IBookShelfProvider
    {
        private readonly IBookShelfDataContext context;

        [PrefferedConstructor]
        public SqLiteDatabaseBookShelfProvider(IBookShelfDataContext context)
        {
            this.context = context;
        }

        public IReadOnlyCollection<AudioBook> GetBooks()
        {
            var books = context.Books
                .Where(book => !book.IsExcluded)
                .Select(book => new AudioBook(book.Id)
                {
                    Title = book.Title
                })
                .AsNoTracking()
                .ToArray();

            return books;
        }
    }
}

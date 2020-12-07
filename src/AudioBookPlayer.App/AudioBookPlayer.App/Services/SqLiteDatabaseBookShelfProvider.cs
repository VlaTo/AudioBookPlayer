using AudioBookPlayer.App.Models;
using LibraProgramming.Xamarin.Dependency.Container.Attributes;
using System;
using System.Collections.Generic;

namespace AudioBookPlayer.App.Services
{
    internal sealed class SqLiteDatabaseBookShelfProvider : IBookShelfProvider
    {
        [PrefferedConstructor]
        public SqLiteDatabaseBookShelfProvider()
        {
        }

        public IReadOnlyCollection<AudioBook> GetBooks()
        {
            return new AudioBook[0];
        }
    }
}

using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Data;
using AudioBookPlayer.App.Models;
using LibraProgramming.Xamarin.Dependency.Container.Attributes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace AudioBookPlayer.App.Services
{
    internal sealed class BookShelfProvider : IBookShelfProvider
    {
        private readonly IPermissionRequestor permissionRequestor;
        private readonly IBookShelfDataContext context;
        private readonly WeakEventManager<AudioBooksEventArgs> queryBooksReady;

        public event EventHandler<AudioBooksEventArgs> QueryBooksReady
        {
            add => queryBooksReady.AddEventHandler(value);
            remove => queryBooksReady.RemoveEventHandler(value);
        }

        [PrefferedConstructor]
        public BookShelfProvider(
            IPermissionRequestor permissionRequestor,
            IBookShelfDataContext context)
        {
            this.permissionRequestor = permissionRequestor;
            this.context = context;

            queryBooksReady = new WeakEventManager<AudioBooksEventArgs>();
        }

        public async Task QueryBooksAsync()
        {
            var status = await permissionRequestor.CheckAndRequestMediaPermissionsAsync();

            if (PermissionStatus.Denied == status)
            {
                return;
            }

            var books = await context.Books
                .Where(book => !book.IsExcluded)
                .Select(book => new AudioBook(book.Id)
                {
                    Title = book.Title
                })
                .AsNoTracking()
                .ToArrayAsync();

            queryBooksReady.RaiseEvent(this, new AudioBooksEventArgs(books), nameof(QueryBooksReady));
        }

        public async Task RefreshBooksAsync()
        {
            var status = await permissionRequestor.CheckAndRequestMediaPermissionsAsync();

            if (PermissionStatus.Denied == status)
            {
                return;
            }

            await QueryBooksAsync();
        }
    }
}

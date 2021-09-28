using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using AudioBookPlayer.App.Android.Core;
using AudioBookPlayer.App.Domain.Models;
using AudioBookPlayer.App.Domain.Services;
using System;
using System.Collections.Generic;

namespace AudioBookPlayer.App.Android.Services.Helpers
{
    internal static class QueueHelper
    {
        public static IEnumerable<MediaSessionCompat.QueueItem> GetLastPlaying()
        {
            return Array.Empty<MediaSessionCompat.QueueItem>();
        }
        
        public static IEnumerable<MediaSessionCompat.QueueItem> GetQueue(IBooksService booksService, EntityId bookId)
        {
            var audioBook = booksService.GetBook(bookId);

            if (null == audioBook)
            {
                return Array.Empty<MediaSessionCompat.QueueItem>();
            }

            var description = new MediaDescriptionCompat.Builder();
            var mediaId = new MediaBookId(bookId);
            
            description.SetMediaId(mediaId.ToString());
            description.SetTitle(audioBook.Title);
            description.SetDescription(audioBook.Synopsis);

            var queueItem = new MediaSessionCompat.QueueItem(description.Build(), (long)bookId);

            return new[] { queueItem };
        }
    }
}
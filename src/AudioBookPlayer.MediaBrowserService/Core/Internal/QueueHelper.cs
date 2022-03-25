﻿using Android.Net;
using Android.OS;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using AudioBookPlayer.Domain;
using AudioBookPlayer.Domain.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AudioBookPlayer.MediaBrowserService.Core.Internal
{
    internal static class QueueHelper
    {
        public static IEnumerable<MediaSessionCompat.QueueItem> BuildQueue(AudioBook audioBook)
        {
            var queue = new Collection<MediaSessionCompat.QueueItem>();

            for (var chapterIndex = 0; chapterIndex < audioBook.Chapters.Count; chapterIndex++)
            {
                var chapter = audioBook.Chapters[chapterIndex];
                var description = new MediaDescriptionCompat.Builder();
                var mediaId = new MediaID(audioBook.MediaId, chapterIndex);

                description.SetTitle(chapter.Title);
                description.SetMediaId(mediaId.ToString());
                description.SetMediaUri(Uri.Parse(chapter.Section.SourceFileUri) ?? Uri.Empty);

                var extra = new Bundle();

                extra.PutLong("Chapter.Offset", (long)chapter.Offset.TotalMilliseconds);
                extra.PutLong("Chapter.Duration", (long)chapter.Duration.TotalMilliseconds);
                extra.PutString("Chapter.Section", chapter.Section.Title);

                description.SetExtras(extra);

                var queueItem = new MediaSessionCompat.QueueItem(description.Build(), queue.Count);

                queue.Add(queueItem);
            }

            return queue.AsEnumerable();
        }
    }
}
﻿using Android.OS;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Domain.Models;
using AudioBookPlayer.App.Domain.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Uri = Android.Net.Uri;

namespace AudioBookPlayer.App.Android.Services.Helpers
{
    internal static class QueueHelper
    {
        public static IEnumerable<MediaSessionCompat.QueueItem> GetLastPlaying()
        {
            return Array.Empty<MediaSessionCompat.QueueItem>();
        }
        
        public static IEnumerable<MediaSessionCompat.QueueItem> BuildQueue(AudioBook audioBook)
        {
            var queue = new Collection<MediaSessionCompat.QueueItem>();

            for (var sectionIndex = 0; sectionIndex < audioBook.Sections.Count; sectionIndex++)
            {
                var section = audioBook.Sections[sectionIndex];
                var mediaUri = Uri.Parse(section.ContentUri);
                var sectionExtra = new Bundle();

                sectionExtra.PutInt("Section.Index", sectionIndex);
                sectionExtra.PutString("Section.Name", section.Name);

                for (var chapterIndex = 0; chapterIndex < section.Chapters.Count; chapterIndex++)
                {
                    var chapter = section.Chapters[chapterIndex];
                    var description = new MediaDescriptionCompat.Builder();
                    var mediaId = new MediaId(audioBook.Id, sectionIndex, chapterIndex);

                    description.SetMediaId(mediaId.ToString());
                    description.SetMediaUri(mediaUri);
                    description.SetTitle(chapter.Title);

                    var extra = new Bundle(sectionExtra);

                    // extra.PutString("SectionId", sectionId);
                    extra.PutDouble("Start", chapter.Start.TotalMilliseconds);
                    extra.PutDouble("Duration", chapter.Duration.TotalMilliseconds);

                    description.SetExtras(extra);

                    var queueItem = new MediaSessionCompat.QueueItem(description.Build(), queue.Count);

                    queue.Add(queueItem);
                }
            }

            return queue.AsEnumerable();
        }
    }
}
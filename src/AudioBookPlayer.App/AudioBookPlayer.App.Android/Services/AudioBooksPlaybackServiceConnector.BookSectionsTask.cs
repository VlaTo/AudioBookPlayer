using Android.OS;
using Android.Support.V4.Media;
using AudioBookPlayer.App.Android.Core;
using AudioBookPlayer.App.Android.Extensions;
using AudioBookPlayer.App.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AudioBookPlayer.App.Android.Services
{
    internal partial class AudioBooksPlaybackServiceConnector
    {
        /// <summary>
        /// The Book Sections task class.
        /// </summary>
        private sealed class BookSectionsTask : TaskExecutor<EntityId, IReadOnlyList<SectionItem>>
        {
            private readonly MediaBrowserCompat mediaBrowser;

            public BookSectionsTask(MediaBrowserCompat mediaBrowser)
                : base(new EntityTaskLock<IReadOnlyList<SectionItem>>())
            {
                this.mediaBrowser = mediaBrowser;
            }

            protected override void DoExecute(EntityId key, TaskCompletionSource<IReadOnlyList<SectionItem>> tcs, Action callback)
            {
                var mediaId = new MediaBookId(key).ToString();
                var subscriptionCallback = new SubscriptionCallback();

                subscriptionCallback.ChildrenLoadedImpl = (parentId, children) =>
                {
                    var sectionItems = new SectionItem[children.Count];

                    for (var index = 0; index < children.Count; index++)
                    {
                        var mediaItem = children[index];
                        sectionItems[index] = mediaItem.ToSectionItem();
                    }

                    tcs.SetResult(sectionItems);

                    callback.Invoke();
                };

                subscriptionCallback.ErrorImpl = parentId =>
                {
                    tcs.TrySetException(new Exception());
                    callback.Invoke();
                };

                var options = new Bundle();

                mediaBrowser.Subscribe(mediaId, options, subscriptionCallback);
            }

            /*public Task<IReadOnlyList<SectionItem>> GetSectionsAsync(EntityId bookId)
            {
                var tcs = new TaskCompletionSource<IReadOnlyList<SectionItem>>();

                if (AcquireTask(bookId, tcs, out var current))
                {
                    var mediaId = new MediaBookId(bookId).ToString();
                    var callback = new SubscriptionCallback();

                    void Unsubscribe()
                    {
                        mediaBrowser.Unsubscribe(mediaId, callback);
                        ReleaseTask(bookId, current);
                    }

                    callback.ChildrenLoadedImpl = (parentId, children) =>
                    {
                        var sectionItems = new SectionItem[children.Count];

                        for (var index = 0; index < children.Count; index++)
                        {
                            var mediaItem = children[index];
                            sectionItems[index] = mediaItem.ToSectionItem();
                        }

                        current.SetResult(sectionItems);

                        Unsubscribe();
                    };

                    callback.ErrorImpl = parentId =>
                    {
                        current.TrySetException(new Exception());
                        Unsubscribe();
                    };

                    var options = new Bundle();

                    mediaBrowser.Subscribe(mediaId, options, callback);
                }

                return current.Task;
            }*/

            /*private bool AcquireTask(
                EntityId bookId,
                TaskCompletionSource<IReadOnlyList<SectionItem>> tcs,
                out TaskCompletionSource<IReadOnlyList<SectionItem>> current)
            {
                lock (gate)
                {
                    if (false == sectionsTasks.TryGetValue(bookId, out var value))
                    {
                        sectionsTasks.Add(bookId, tcs);
                        current = tcs;

                        return true;
                    }

                    current = value;

                    return false;
                }
            }*/

            /*private void ReleaseTask(EntityId bookId, TaskCompletionSource<IReadOnlyList<SectionItem>> tcs)
            {
                lock (gate)
                {
                    if (sectionsTasks.TryGetValue(bookId, out var value) && tcs == value)
                    {
                        sectionsTasks.Remove(bookId);
                    }
                }
            }*/
        }
    }
}
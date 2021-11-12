using Android.OS;
using Android.Support.V4.Media;
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
                var mediaId = new MediaId(key).ToString();
                var subscriptionCallback = new SubscriptionCallback
                {
                    OnChildrenLoadedImpl = (parentId, children, options) =>
                    {
                        var sectionItems = new SectionItem[children.Count];

                        for (var index = 0; index < children.Count; index++)
                        {
                            var mediaItem = children[index];
                            sectionItems[index] = mediaItem.ToSectionItem();
                        }

                        tcs.SetResult(sectionItems);

                        callback.Invoke();
                    },
                    OnErrorImpl = (parentId, options) =>
                    {
                        tcs.TrySetException(new Exception());
                        callback.Invoke();
                    }
                };

                var options = new Bundle();

                mediaBrowser.Subscribe(mediaId, options, subscriptionCallback);
            }
        }
    }
}
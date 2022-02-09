#nullable enable

using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Java.Lang;
using System;
using System.Collections.Concurrent;
using Object = Java.Lang.Object;
using String = System.String;
using Uri = Android.Net.Uri;

namespace AudioBookPlayer.Core
{
    /// <summary>
    /// 
    /// </summary>
    public static class LoadImage
    {
        public static LoadImage<T> For<T>() where T : Object
        {
            var service = ImageContentService.GetInstance();
            var cache = ImageCache.GetInstance();
            return new LoadImage<T>(service, cache);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class LoadImage<T> : HandlerThread, Handler.ICallback
        where T : Object
    {
        private static int counter;
        private readonly ImageContentService service;
        private readonly ImageCache cache;
        private readonly ConcurrentDictionary<T, HandlerInfo> map;
        private Handler? responseHandler;
        private Handler? requestHandler;

        public LoadImage(ImageContentService service, ImageCache cache)
            : base($"{nameof(LoadImage)}#{++counter}")
        {
            this.service = service;
            this.cache = cache;

            map = new ConcurrentDictionary<T, HandlerInfo>();
        }

        static LoadImage()
        {
            counter = 0;
        }

        public void QueueImage(T target, Uri contentUri, Action<T, Uri, Bitmap?, Bitmap?> callback)
        {
            if (null == requestHandler)
            {
                return;
            }

            if (map.TryAdd(target, new HandlerInfo(contentUri, callback)))
            {
                var message = requestHandler.ObtainMessage(0, target);
                message.SendToTarget();
            }
        }

        public void ClearQueue()
        {
            if (null == requestHandler)
            {
                return;
            }

            requestHandler.RemoveMessages(0);
        }

        protected override void OnLooperPrepared()
        {
            base.OnLooperPrepared();

            requestHandler = new Handler(Looper.MyLooper()!, this);
            responseHandler = new Handler(Looper.MainLooper!);
        }

        private void HandleRequest(T target)
        {
            if (map.TryGetValue(target, out var info) && null != responseHandler)
            {
                var contentUri = info.ContentUri;

                if (false == cache.Contains(contentUri))
                {
                    var stream = service.GetImageStream(contentUri);
                    var original = BitmapFactory.DecodeStream(stream);

                    if (null != original)
                    {
                        // book sheet proportion 1.38-1.64 (~1.6)
                        var thumbnail = Bitmap.CreateScaledBitmap(original, 60, 82, true);
                        var cover = Bitmap.CreateScaledBitmap(original, 280, 384, true);

                        cache.PutImages(contentUri, new ImageCache.ImageEntry(thumbnail, cover));
                    }
                }

                responseHandler.Post(new Runnable(() =>
                {
                    if (map.TryGetValue(target, out var handlerInfo) && String.Equals(contentUri, handlerInfo.ContentUri) && map.TryRemove(target, out _))
                    {
                        if (cache.TryGetImages(contentUri, out var entry))
                        {
                            handlerInfo.Callback.Invoke(target, contentUri, entry.Thumbnail, entry.Cover);
                        }
                    }
                    else
                    {
                        handlerInfo.Callback.Invoke(target, contentUri, null, null);
                    }
                }));
            }
        }

        #region Handler.ICallback

        bool Handler.ICallback.HandleMessage(Message msg)
        {
            if (0 == msg.What)
            {
                var target = msg.Obj.JavaCast<T>();

                if (null != target)
                {
                    HandleRequest(target);
                }

                return true;
            }

            return false;
        }

        #endregion

        /// <summary>
        /// HandlerInfo class.
        /// </summary>
        private readonly struct HandlerInfo
        {
            public Uri ContentUri
            {
                get;
            }

            public Action<T, Uri, Bitmap?, Bitmap?> Callback
            {
                get;
            }

            public HandlerInfo(Uri contentUri, Action<T, Uri, Bitmap?, Bitmap?> callback)
            {
                ContentUri = contentUri;
                Callback = callback;
            }
        }
    }
}
#nullable enable

using System.Collections.Concurrent;
using Android.Content;
using Android.Graphics;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AudioBookPlayer.Core;
using Java.Lang;
using Java.Util.Concurrent;

namespace AudioBookPlayer.App.Core
{
    internal sealed class BookImageTask<T> : HandlerThread, Handler.ICallback
        where T : Object
    {
        private readonly ImageContentService imageContentService;
        private readonly ImageCache cache;
        private readonly ConcurrentDictionary<T, Uri> map;
        private Handler? responseHandler;
        private Handler? requestHandler;

        public IBookImageListener? Listener
        {
            get;
            set;
        }

        public BookImageTask(ImageContentService imageContentService, ImageCache cache)
            : base(nameof(BookImageTask<T>))
        {
            this.imageContentService = imageContentService;
            this.cache = cache;
            map = new ConcurrentDictionary<T, Uri>();
        }

        public void QueueImage(T target, Uri contentUri)
        {
            if (map.TryAdd(target, contentUri))
            {
                var message = requestHandler.ObtainMessage(0, target);
                message.SendToTarget();
            }
        }

        protected override void OnLooperPrepared()
        {
            base.OnLooperPrepared();

            requestHandler = new Handler(Looper.MyLooper(), this);
            responseHandler = new Handler(Looper.MainLooper);
        }

        private void HandleRequest(T target)
        {
            if (map.TryGetValue(target, out var contentUri) && null != responseHandler)
            {
                if (false == cache.TryGetImages(contentUri, out var thumbnail, out var cover))
                {
                    var stream = imageContentService.GetImageStream(contentUri);
                    var original = BitmapFactory.DecodeStream(stream);

                    if (null != original)
                    {
                        thumbnail = Bitmap.CreateScaledBitmap(original, 48, 76, true);
                        cover = Bitmap.CreateScaledBitmap(original, 240, 384, true);

                        cache.PutImages(contentUri, thumbnail, cover);
                    }
                }

                responseHandler.Post(new Runnable(() =>
                {
                    if (map.TryGetValue(target, out var temp) && contentUri == temp && map.TryRemove(target, out _))
                    {
                        if (cache.TryGetImages(contentUri, out var image, out _))
                        {
                            if (null != Listener)
                            {
                                Listener.OnImageLoaded(target, image);
                            }
                        }
                    }
                }));
            }
        }

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

        public interface IBookImageListener
        {
            void OnImageLoaded(T target, Bitmap? bitmap);
        }
    }
}
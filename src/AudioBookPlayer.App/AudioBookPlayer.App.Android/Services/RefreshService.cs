#nullable enable

using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.OS;
using ResultReceiver = Android.OS.ResultReceiver;

namespace AudioBookPlayer.App.Android.Services
{
    // Activity to service communication with result back propagation.
    // source from https://habr.com/ru/post/167679/

    /*
    public class RefreshService : Service
    {
        public override void OnCreate()
        {
            base.OnCreate();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        public override void OnStart(Intent? intent, int startId)
        {
            base.OnStart(intent, startId);
        }

        public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
        {
            var resultReceiver = (IResultReceiver?)intent?.GetParcelableExtra("Extra");

            if (null != resultReceiver)
            {
                var result = Bundle.Empty;
                resultReceiver.Send(0, result);
            }

            return base.OnStartCommand(intent, flags, startId);
        }

        public override IBinder? OnBind(Intent? intent)
        {
            return base.OnBind(intent);
        }
    }

    public class AppResultReceiver : ResultReceiver
    {
        private IReceiver? receiver;

        public interface IReceiver
        {
            void OnReceiveResult(int resultCode, Bundle? data);
        }

        public AppResultReceiver(Handler? handler)
            : base(handler)
        {
        }

        public void SetReceiver(IReceiver? value)
        {
            receiver = value;
        }

        protected override void OnReceiveResult(int resultCode, Bundle? resultData)
        {
            if (null != receiver)
            {
                receiver.OnReceiveResult(resultCode, resultData);
            }
        }
    }

    public class MainActivity : Activity, AppResultReceiver.IReceiver
    {
        private AppResultReceiver receiver;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            void OnMessage(Message message)
            {
                ;
            }

            receiver = new AppResultReceiver(new Handler(OnMessage));
            receiver.SetReceiver(this);

            var intent = new Intent("ACTION_TEST", null, Application.Context, Java.Lang.Class.FromType(typeof(RefreshService)));
            intent.PutExtra("RECEIVER", receiver);

            StartService(intent);
        }

        public void OnReceiveResult(int resultCode, Bundle? data)
        {
            switch (resultCode)
            {
                case 0:
                {
                    if (null != data)
                    {
                        ;
                    }

                    break;
                }

                default:
                {
                    break;
                }
            }
        }
    }
    */
}
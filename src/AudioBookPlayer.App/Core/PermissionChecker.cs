using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content.PM;
using Android.Views;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using Google.Android.Material.Snackbar;

namespace AudioBookPlayer.App.Core
{
    public sealed class PermissionChecker
    {
        public const int DefaultRequestCode = 0;
        public const int RationaleShownRequestCode = 1;

        private static PermissionChecker instance;

        private readonly Activity activity;
        private readonly List<Action<int, string[], Permission[]>> callbacks;

        private PermissionChecker(Activity activity)
        {
            this.activity = activity;
            callbacks = new List<Action<int, string[], Permission[]>>();
        }

        public static void Init(Activity activity)
        {
            if (null == instance)
            {
                instance = new PermissionChecker(activity);
            }
        }

        public static void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            if (null == instance)
            {
                throw new ApplicationException();
            }

            instance.OnRequestPermissionsResultInternal(requestCode, permissions, grantResults);
        }

        public static void CheckPermissions(View view, string[] permissions, Action<int, string[], Permission[]> callback)
        {
            if (null == instance)
            {
                throw new ApplicationException();
            }

            instance.CheckPermissionsInternal(view, permissions, callback);
        }

        private void CheckPermissionsInternal(View view, string[] permissions, Action<int, string[], Permission[]> callback)
        {
            var pendingPermissions = new List<string>();

            for (var index = 0; index < permissions.Length; index++)
            {
                var permission = ContextCompat.CheckSelfPermission(Application.Context, permissions[index]);

                if (Permission.Granted == permission)
                {
                    continue;
                }

                pendingPermissions.Add(permissions[index]);
            }

            if (0 == pendingPermissions.Count)
            {
                var grantResults = Enumerable.Repeat(Permission.Granted, permissions.Length).ToArray();

                callback.Invoke(DefaultRequestCode, permissions, grantResults);

                return;
            }

            var requiredPermissions = pendingPermissions.ToArray();
            var shouldShow = requiredPermissions.Any(
                permission => ActivityCompat.ShouldShowRequestPermissionRationale(activity, permission)
            );

            callbacks.Add(callback);

            if (false == shouldShow)
            {
                ActivityCompat.RequestPermissions(activity, requiredPermissions, DefaultRequestCode);
                return;
            }

            Snackbar
                .Make(
                    Application.Context,
                    view,
                    "Request permisison for update",
                    Snackbar.LengthIndefinite
                )
                .SetAction(
                    "Request",
                    _ => ActivityCompat.RequestPermissions(activity, requiredPermissions, RationaleShownRequestCode)
                )
                .Show();
        }

        private void OnRequestPermissionsResultInternal(int requestCode, string[] permissions, Permission[] grantResults)
        {
            var handlers = callbacks.ToArray();

            callbacks.Clear();

            for (var index = 0; index < handlers.Length; index++)
            {
                var handler = handlers[index];

                if (null == handler)
                {
                    continue;
                }

                handler.Invoke(requestCode, permissions, grantResults);
            }
        }
    }
}
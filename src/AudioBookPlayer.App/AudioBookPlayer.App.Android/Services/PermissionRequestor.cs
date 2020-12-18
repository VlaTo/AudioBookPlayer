using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AudioBookPlayer.App.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using static Xamarin.Essentials.Permissions;

namespace AudioBookPlayer.App.Droid.Services
{
    public class ReadExternalStorage : BasePlatformPermission
    {
        //[TupleElementNames(new[] { "androidPermission", "isRuntime" })]
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions
        {
            get
            {
                var temp = base.RequiredPermissions;
                return new[]
                {
                    (Android.Manifest.Permission.ReadExternalStorage, true)
                };
            }
        }
    }

    internal sealed class PermissionRequestor : IPermissionRequestor
    {
        public PermissionRequestor()
        {

        }

        public Task<PermissionStatus> CheckAndRequestMediaPermissionsAsync()
        {
            return CheckAndRequestPermissionAsync<ReadExternalStorage>();
        }

        private static async Task<PermissionStatus> CheckAndRequestPermissionAsync<TPermission>()
            where TPermission : BasePermission, new()
        {
            var status = await Permissions.CheckStatusAsync<TPermission>();

            if (PermissionStatus.Granted == status)
            {
                return status;
            }

            if (PermissionStatus.Denied == status && DevicePlatform.iOS == DeviceInfo.Platform)
            {
                return status;
            }

            status = await Permissions.RequestAsync<TPermission>();

            return status;
        }
    }
}
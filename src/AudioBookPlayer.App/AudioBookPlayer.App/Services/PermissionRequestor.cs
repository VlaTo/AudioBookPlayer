using System.Threading.Tasks;
using Xamarin.Essentials;
using static Xamarin.Essentials.Permissions;

namespace AudioBookPlayer.App.Services
{
    /*public class ReadExternalStorage : BasePlatformPermission
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
    }*/

    /*internal sealed class PermissionRequestor : IPermissionRequestor
    {
        public PermissionRequestor()
        {
        }

        public Task<PermissionStatus> CheckAndRequestMediaPermissionsAsync()
        {
            return CheckAndRequestPermissionAsync(new Permissions.Media());
            //return CheckAndRequestPermissionAsync(new BasePermission();
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

            //status = await Permissions.RequestAsync<TPermission>();

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                status = await Permissions.RequestAsync<TPermission>();
            });

            return status;
        }

        private static async Task<PermissionStatus> CheckAndRequestPermissionAsync<T>(T permission)
            where T : BasePermission
        {
            var status = await permission.CheckStatusAsync();

            if (PermissionStatus.Granted == status)
            {
                return status;
            }

            if (PermissionStatus.Denied == status && DevicePlatform.iOS == DeviceInfo.Platform)
            {
                return status;
            }

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                status = await permission.RequestAsync();
            });

            return status;
        }
    }*/
}

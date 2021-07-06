using AudioBookPlayer.App.Services;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Permission = Android.Manifest.Permission;

namespace AudioBookPlayer.App.Android.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class ReadExternalStoragePermission : Permissions.BasePlatformPermission
    {
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions
        {
            get;
        }

        public ReadExternalStoragePermission()
        {
            RequiredPermissions = new[]
            {
                (Permission.ReadExternalStorage, true)
            };
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal sealed class PermissionRequestor : IPermissionRequestor
    {
        public Task<PermissionStatus> CheckAndRequestMediaPermissionsAsync()
        {
            return CheckAndRequestPermissionAsync<ReadExternalStoragePermission>();
        }

        private static async Task<PermissionStatus> CheckAndRequestPermissionAsync<TPermission>()
            where TPermission : Permissions.BasePermission, new()
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
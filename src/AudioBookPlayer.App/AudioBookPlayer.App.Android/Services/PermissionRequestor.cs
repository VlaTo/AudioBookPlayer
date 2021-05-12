using AudioBookPlayer.App.Services;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace AudioBookPlayer.App.Droid.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class ReadExternalStorage : Permissions.BasePlatformPermission
    {
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions
        {
            get;
        }

        public ReadExternalStorage()
        {
            RequiredPermissions = new[]
            {
                (Android.Manifest.Permission.ReadExternalStorage, true)
                //(Android.Manifest.Permission.MediaContentControl, true)
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
            return CheckAndRequestPermissionAsync<ReadExternalStorage>();
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
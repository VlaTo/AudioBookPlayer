using System.Threading.Tasks;
using AudioBookPlayer.App.Services;
using Xamarin.Essentials;
using Permission = Android.Manifest.Permission;

namespace AudioBookPlayer.App.Android.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class ExternalMediaStoragePermissions : Permissions.BasePlatformPermission
    {
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions
        {
            get;
        }

        public ExternalMediaStoragePermissions()
        {
            RequiredPermissions = new[]
            {
                (Permission.ReadExternalStorage, true),
                (Permission.WriteExternalStorage, true),
                (Permission.ManageExternalStorage, true)
            };
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal sealed class PermissionRequestor : IPermissionRequestor
    {
        public Task<PermissionStatus> CheckAndRequestMediaPermissionsAsync()
            => CheckAndRequestPermissionAsync<ExternalMediaStoragePermissions>();

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
using System.Threading.Tasks;
using Xamarin.Essentials;
using static Xamarin.Essentials.Permissions;

namespace AudioBookPlayer.App.Services
{
    internal sealed class PermissionRequestor : IPermissionRequestor
    {
        public PermissionRequestor()
        {
        }

        public Task<PermissionStatus> CheckAndRequestMediaPermissionsAsync()
        {
            return CheckAndRequestPermissionAsync(new Permissions.Media());
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

            status = await permission.RequestAsync();

            return status;
        }
    }
}

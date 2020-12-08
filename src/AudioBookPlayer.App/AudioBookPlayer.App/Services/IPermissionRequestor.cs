using System.Threading.Tasks;
using Xamarin.Essentials;

namespace AudioBookPlayer.App.Services
{
    public interface IPermissionRequestor
    {
        Task<PermissionStatus> CheckAndRequestMediaPermissionsAsync();
    }
}

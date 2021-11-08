using System.Threading.Tasks;
using LibraProgramming.Xamarin.Interaction;
using Xamarin.Essentials;

namespace AudioBookPlayer.App.ViewModels.RequestContexts
{
    public sealed class CheckPermissionsRequestContext : InteractionRequestContext
    {
        private readonly TaskCompletionSource<PermissionStatus> tcs;

        public CheckPermissionsRequestContext()
        {
            tcs = new TaskCompletionSource<PermissionStatus>();
        }

        public Task<PermissionStatus> WaitAsync() => tcs.Task;

        public void SetStatus(PermissionStatus status) => tcs.TrySetResult(status);
    }
}
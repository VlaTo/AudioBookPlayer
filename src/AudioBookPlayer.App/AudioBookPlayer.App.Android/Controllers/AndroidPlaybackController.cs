using AudioBookPlayer.App.Core.Controllers;
using AudioBookPlayer.App.Droid.Controllers;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidPlaybackController))]

namespace AudioBookPlayer.App.Droid.Controllers
{
    internal sealed class AndroidPlaybackController : IPlaybackController
    {
    }
}
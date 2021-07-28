using AudioBookPlayer.App.Domain.Models;

namespace AudioBookPlayer.App.Services
{
    public interface IRemoteControlService
    {
        void ShowInformation(AudioBook audioBook);

        void HideInformation();
    }
}
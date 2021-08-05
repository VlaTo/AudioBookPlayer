using AudioBookPlayer.App.Domain.Models;

namespace AudioBookPlayer.App.Services
{
    public interface INotificationService
    {
        void ShowInformation(AudioBook audioBook);

        void HideInformation();
    }
}
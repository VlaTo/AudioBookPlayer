namespace AudioBookPlayer.App.Core.Services
{
    public interface IPlaybackControlService
    {
        void ShowNotification();

        void StartPlay(string filename);
    }
}
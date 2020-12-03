namespace AudioBookPlayer.App.Core.Services
{
    public interface IPlaybackController
    {
        void ShowNotification();

        void StartPlay(string filename);
    }
}
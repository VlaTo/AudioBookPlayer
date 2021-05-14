namespace AudioBookPlayer.App.Services
{
    public interface IAudioBookFactoryProvider
    {
        IAudioBookFactory CreateFactoryFor(string extension);
    }
}
namespace AudioBookPlayer.MediaBrowserService.Core
{
    public interface IProgressEx<in TKey, in TProgress> where TProgress : struct
    {
        void Report(TKey key, TProgress progress);
    }
}
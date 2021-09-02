namespace AudioBookPlayer.App.Services
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMediaInfoProviderFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="extension"></param>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        IMediaInfoProvider CreateProviderFor(string extension, string mimeType);
    }
}
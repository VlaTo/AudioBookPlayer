namespace AudioBookPlayer.App.Services
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPlatformToastService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        void ShowLongMessage(string message);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        void ShowShortMessage(string message);
    }
}
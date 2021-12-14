namespace AudioBookPlayer.App.Core
{
    internal sealed partial class MediaBrowserServiceConnector
    {
        /// <summary>
        /// 
        /// </summary>
        internal interface IConnectCallback
        {
            void OnConnected(IMediaBrowserService service);

            void OnSuspended();

            void OnFailed();
        }
    }
}
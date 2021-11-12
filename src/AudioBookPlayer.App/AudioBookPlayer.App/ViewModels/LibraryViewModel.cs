using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Services;
using AudioBookPlayer.App.ViewModels.RequestContexts;
using LibraProgramming.Xamarin.Dependency.Container.Attributes;
using LibraProgramming.Xamarin.Interaction;
using System.Threading.Tasks;
using LibraProgramming.Xamarin.Interaction.Contracts;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AudioBookPlayer.App.ViewModels
{
    public sealed class LibraryViewModel : ViewModelBase, IInitialize
    {
        private readonly IMediaBrowserServiceConnector connector;
        private readonly ITaskExecutionMonitor libraryConnect;
        private bool isBusy;

        public bool IsBusy
        {
            get => isBusy;
            set => SetProperty(ref isBusy, value);
        }

        public Command UpdateLibrary
        {
            get;
        }

        public InteractionRequest<CheckPermissionsRequestContext> CheckPermissionsRequest
        {
            get;
        }

        [PrefferedConstructor]
        public LibraryViewModel(IMediaBrowserServiceConnector connector)
        {
            this.connector = connector;

            libraryConnect = new TaskExecutionMonitor(this.connector.ConnectAsync);
            
            var libraryUpdate = new TaskExecutionMonitor(DoUpdateLibraryAsync);
            UpdateLibrary = new Command(libraryUpdate.Start);
            CheckPermissionsRequest = new InteractionRequest<CheckPermissionsRequestContext>();
        }

        void IInitialize.OnInitialize() => libraryConnect.Start();

        private async Task DoUpdateLibraryAsync()
        {
            IsBusy = true;

            try
            {
                System.Diagnostics.Debug.WriteLine("[LibraryViewModel] [DoUpdateLibraryAsync] Update begin");

                var context = new CheckPermissionsRequestContext();
                
                CheckPermissionsRequest.Raise(context);

                var status = await context.WaitAsync();

                if (PermissionStatus.Granted == status)
                {
                    await connector.UpdateLibraryAsync();

                    System.Diagnostics.Debug.WriteLine("[LibraryViewModel] [DoUpdateLibraryAsync] Update complete");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}

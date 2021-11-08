using System;
using AudioBookPlayer.App.Core;
using AudioBookPlayer.App.Services;
using LibraProgramming.Xamarin.Dependency.Container.Attributes;
using System.Threading.Tasks;
using AudioBookPlayer.App.ViewModels.RequestContexts;
using LibraProgramming.Xamarin.Interaction;
using LibraProgramming.Xamarin.Interaction.Contracts;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AudioBookPlayer.App.ViewModels
{
    public sealed class LibraryViewModel : ViewModelBase
    {
        private readonly IMediaBrowserServiceConnector connector;
        private readonly IUpdateLibraryService updateLibraryService;
        private readonly ITaskExecutionMonitor libraryUpdateMonitor;
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
        public LibraryViewModel(
            IMediaBrowserServiceConnector connector,
            IUpdateLibraryService updateLibraryService
            )
        {
            this.connector = connector;
            this.updateLibraryService = updateLibraryService;

            libraryUpdateMonitor = new TaskExecutionMonitor(DoUpdateLibraryAsync);

            UpdateLibrary = new Command(DoUpdateLibrary);
            CheckPermissionsRequest = new InteractionRequest<CheckPermissionsRequestContext>();
        }

        private void DoUpdateLibrary()
        {
            libraryUpdateMonitor.Start();
        }

        private async Task DoUpdateLibraryAsync()
        {
            IsBusy = true;

            try
            {
                var context = new CheckPermissionsRequestContext();
                
                CheckPermissionsRequest.Raise(context);

                var status = await context.WaitAsync();

                if (PermissionStatus.Granted == status)
                {
                    var progress = new Progress<int>();
                    progress.ProgressChanged += DoUpdateProgressChanged;

                    updateLibraryService.Update(progress);

                    MessagingCenter.Send(this, "1", true);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void DoUpdateProgressChanged(object _, int progress)
        {
            ;
        }

        /*private void DoUpdateLibrary()
        {
            updateLibraryService.StartUpdate();

            var status = await permissions.CheckAndRequestMediaPermissionsAsync();

            if (PermissionStatus.Denied == status)
            {
                return;
            }

            refreshExecution.Start();
        }*/
    }
}

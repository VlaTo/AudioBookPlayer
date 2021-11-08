using System;
using System.Threading.Tasks;
using AudioBookPlayer.App.Core.Attributes;
using AudioBookPlayer.App.Core.Extensions;
using AudioBookPlayer.App.Services;
using AudioBookPlayer.App.ViewModels;
using AudioBookPlayer.App.ViewModels.RequestContexts;
using Xamarin.Essentials;
using Xamarin.Forms.Xaml;

namespace AudioBookPlayer.App.Views
{
    [ViewModel(typeof(LibraryViewModel))]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LibraryPage
    {
        private readonly IPermissionRequestor permissionRequestor;

        public LibraryPage()
        {
            InitializeComponent();

            permissionRequestor = AudioBookPlayerApplication.Instance.DependencyContainer.GetInstance<IPermissionRequestor>();
        }

        private void OnCheckPermissionsRequest(object _, CheckPermissionsRequestContext context, Action callback)
        {
            MainThread
                .InvokeOnMainThreadAsync(DoCheckPermissionsAsync(context))
                .RunAndForget();
        }

        private Func<Task> DoCheckPermissionsAsync(CheckPermissionsRequestContext context)
        {
            return async () =>
            {
                var status = await permissionRequestor.CheckAndRequestMediaPermissionsAsync();
                context.SetStatus(status);
            };
        }
    }
}
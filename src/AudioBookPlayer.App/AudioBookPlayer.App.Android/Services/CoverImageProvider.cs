using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Android.Content;
using Android.Net;
using AudioBookPlayer.App.Android.Services;
using AudioBookPlayer.App.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(CoverImageProvider))]

namespace AudioBookPlayer.App.Android.Services
{
    public class CoverImageProvider : ICoverImageProvider
    {
        private const string ReadMode = "r";
        
        private readonly ContentResolver resolver;

        public CoverImageProvider()
        {
            resolver = global::Android.App.Application.Context.ContentResolver;
        }

        public Task<Stream> LoadStreamAsync(string contentUri, CancellationToken cancellationToken = default)
        {
            var uri = Uri.Parse(contentUri);
            var afd = resolver.OpenAssetFileDescriptor(uri, ReadMode);

            return Task.FromResult(afd.CreateInputStream());
        }
    }
}
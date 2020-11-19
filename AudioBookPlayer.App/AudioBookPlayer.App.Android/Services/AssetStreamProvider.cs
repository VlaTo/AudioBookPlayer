using Android.OS;
using AndroidX.Core.Content;
using AudioBookPlayer.App.Core.Services;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace AudioBookPlayer.App.Droid.Services
{
    public class AssetStreamProvider : ISourceStreamProvider
    {
        public string GetBookPath()
        {
            //var permission = ContextCompat.CheckSelfPermission(MainApplication.Context.ApplicationContext, Android.Manifest.Permission.ReadExternalStorage);

            //if (permission == Android.Content.PM.Permission.Denied)
            //{

            //}

            return Environment.GetExternalStoragePublicDirectory(Environment.DirectoryDownloads).AbsolutePath;
            //Android.OS.Environment.DirectoryDownloads;
        }

        public async Task<Stream> GetStreamAsync()
        {
            var memory = new MemoryStream();

            using (var source = await FileSystem.OpenAppPackageFileAsync("book.m4b"))
            {
                await source.CopyToAsync(memory);
            }

            return memory;
        }
    }
}
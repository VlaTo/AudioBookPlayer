using AudioBookPlayer.App.Core.Services;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace AudioBookPlayer.App.Droid.Services
{
    public class AssetStreamProvider : ISourceStreamProvider
    {
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
using AudioBookPlayer.App.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AudioBookPlayer.App.Extensions
{
    internal static class AudioBookExtensions
    {
        public static async Task<ImageSource> GetImageAsync(this AudioBook audioBook, string key, CancellationToken cancellationToken = default)
        {
            if (null == audioBook)
            {
                throw new ArgumentNullException(nameof(audioBook));
            }

            var image = audioBook.Images.FirstOrDefault(item => String.Equals(key, item.Key));

            if (null == image)
            {
                return null;
            }

            var imageSource = await image.GetStreamSync(cancellationToken);

            return ImageSource.FromStream(() => imageSource);
        }
    }
}
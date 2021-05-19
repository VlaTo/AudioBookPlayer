using System;
using System.Globalization;
using System.IO;
using Xamarin.Forms;

namespace AudioBookPlayer.App.Core
{
    public sealed class ImageBlobConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (typeof(ImageSource) == targetType)
            {
                if (value is byte[] blob)
                {
                    return ImageSource.FromStream(() => new MemoryStream(blob));
                }
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
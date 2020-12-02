using System;
using System.Globalization;
using Xamarin.Forms;

namespace AudioBookPlayer.App.Converters
{
    public sealed class DoubleToTimeSpanConverter : IValueConverter
    {
        public static readonly Type TargetType = typeof(TimeSpan);
        public static readonly Type SourceType = typeof(double);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            /*if (SourceType.IsAssignableFrom(value.GetType()))
            {
                var time = TimeSpan.FromMilliseconds((double)value);
                return time.ToString("hh:mm:ss", culture);
                //return String.Format(culture, "{0:hh':'mm':'ss}", time);
            }*/

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}

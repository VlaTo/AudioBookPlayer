using System;
using System.Globalization;
using Xamarin.Forms;
using String = System.String;

namespace AudioBookPlayer.App.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class TimeSpanConverter : IValueConverter
    {
        private const string ShortTimeSpanFormat = @"hh\:mm\:ss";
        private const string LongTimeSpanFormat = @"ddd\.hh\:mm\:ss";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (typeof(String) == targetType)
            {
                if (null != value && value is TimeSpan timespan)
                {
                    return ConvertTimeSpanToString(timespan, parameter, culture);
                }
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static object ConvertTimeSpanToString(TimeSpan value, object parameter, CultureInfo culture)
        {
            var format = 0 < value.Days ? LongTimeSpanFormat : ShortTimeSpanFormat;
            return value.ToString(format, culture);
        }
    }
}
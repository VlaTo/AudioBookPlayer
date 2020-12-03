using System;
using System.Globalization;
using Xamarin.Forms;

namespace AudioBookPlayer.App.Converters
{
    public sealed class DoubleToTimeSpanConverter : IValueConverter
    {
        public static readonly Type SourceType = typeof(TimeSpan);
        public static readonly Type TargetType = typeof(String);

        public string MinutesFormat
        {
            get;
            set;
        }

        public string HoursFormat
        {
            get;
            set;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (SourceType.IsAssignableFrom(value.GetType()))
            {
                var timespan = (TimeSpan)value;

                if (TargetType.IsAssignableFrom(targetType))
                {
                    var text = 0 != timespan.Hours
                    ? timespan.ToString(HoursFormat, culture)
                    : timespan.ToString(MinutesFormat, culture);

                    if (TimeSpan.Zero > timespan)
                    {
                        return '-' + text;
                    }

                    return text;
                }
            }

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}

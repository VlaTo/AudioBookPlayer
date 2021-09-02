using System;
using System.Globalization;
using Xamarin.Forms;

namespace AudioBookPlayer.App.Core
{
    public sealed class FileSizeConverter : IValueConverter
    {
        private LongConverter longConverter;

        public FileSizeConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (typeof(String) == targetType)
            {
                if (typeof(long) == value.GetType())
                {
                    if (null == longConverter)
                    {
                        longConverter = new LongConverter(culture);
                    }

                    return longConverter.Convert((long)value);
                }
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private sealed class LongConverter
        {
            private readonly (int Radix, string Suffix)[] scales;
            private readonly CultureInfo culture;

            public LongConverter(CultureInfo culture)
            {
                this.culture = culture;

                scales = new[]
                {
                    ( 1, "B" ),
                    ( 1024, "KB" ),
                    ( 1024, "MB" ),
                    ( 1024, "GB" ),
                    ( 1024, "TB" )
                };
            }

            public string Convert(long value)
            {
                var index = 0;
                var last = scales.Length - 1;

                for (; index < scales.Length; index++)
                {
                    if (scales[index].Radix > value)
                    {
                        if (0 == index)
                        {
                            return value.ToString(culture);
                        }

                        break;
                    }

                    if (last == index)
                    {
                        return String.Format(culture, "{0:N} {1}", value, scales[index].Suffix);
                    }

                    value /= scales[index].Radix;
                }

                return String.Format(culture, "{0:G} {1}", value, scales[index].Suffix);
            }
        }
    }
}

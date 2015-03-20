using System;
using System.Globalization;
using System.Windows.Data;

namespace HashHelper.Converters
{
    public class ByteCountToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            long bytes = 0;

            if (value is int)
            {
                var i = (int)value;
                bytes = i;
            }

            if (value is long)
            {
                bytes = (long)value;
            }

            String size = FileSize.GetSizeString(bytes, "MB", 2);
            return size;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

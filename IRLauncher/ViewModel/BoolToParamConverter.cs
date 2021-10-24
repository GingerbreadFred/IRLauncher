using System;
using System.Globalization;
using System.Windows.Data;

namespace IRLauncher
{
    public class BoolToParamConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool && parameter.GetType().IsArray)
            {
                return (bool)value ? (parameter as object[])[1] : (parameter as object[])[0];
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}

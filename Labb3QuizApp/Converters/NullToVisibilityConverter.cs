using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Labb3QuizApp.Converters
{
    // Could have used a BoolToVisibilityConverter. But I thought about it after I created this.
    internal class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

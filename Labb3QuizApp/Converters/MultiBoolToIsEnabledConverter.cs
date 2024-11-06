using System.Globalization;
using System.Windows.Data;

namespace Labb3QuizApp.Converters
{
    class MultiBoolToIsEnabledConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 && values[0] is bool isEditMode && values[1] is bool hasQuestions)
            {
                return isEditMode && hasQuestions;
            }
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

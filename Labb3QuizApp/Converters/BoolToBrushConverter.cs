using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Labb3QuizApp.Converters
{
    public class BoolToBrushConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 4)
                return Brushes.Transparent;

            var selectedAnswer = values[0]?.ToString();
            var correctAnswer = values[1]?.ToString();
            var answerContent = values[2]?.ToString();
            var isAnswered = values[3] as bool? ?? false;

            if (isAnswered)
            {
                if (answerContent == correctAnswer)
                    return Brushes.LightGreen;
                else if (answerContent == selectedAnswer)
                    return Brushes.OrangeRed;
            }

            return Brushes.Transparent;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

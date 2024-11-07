using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Labb3QuizApp.Converters
{
    public class AnswerToImageConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var selectedAnswer = values[0] as string;
            var correctAnswer = values[1] as string;
            var content = values[2] as string;
            var isAnswered = values.Length > 3 && values[3] is bool && (bool)values[3];

            if (isAnswered)
            {
                if (content == correctAnswer)
                {
                    return new BitmapImage(new Uri("pack://application:,,,/Assets/correct.png"));
                }
                else if (content == selectedAnswer)
                {
                    return new BitmapImage(new Uri("pack://application:,,,/Assets/incorrect.png"));
                }
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
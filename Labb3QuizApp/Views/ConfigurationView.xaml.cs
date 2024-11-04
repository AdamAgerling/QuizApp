using Labb3QuizApp.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace Labb3QuizApp.Views
{

    public partial class ConfigurationView : UserControl
    {
        public ConfigurationView()
        {
            InitializeComponent();
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var viewModel = (ConfigurationViewModel)DataContext;
            viewModel.UpdateQuestion();
        }
    }
}

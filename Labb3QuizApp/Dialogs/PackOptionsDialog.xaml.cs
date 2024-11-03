using Labb3QuizApp.ViewModel;
using System.Windows;

namespace Labb3QuizApp.Dialogs
{
    public partial class PackOptionsDialog : Window
    {
        public PackOptionsDialog()
        {
            InitializeComponent();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ConfigurationViewModel viewModel)
            {
                viewModel.UpdatePack();
                DialogResult = true;
            }
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}


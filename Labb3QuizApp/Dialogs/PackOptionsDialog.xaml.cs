using Labb3QuizApp.Model;
using Labb3QuizApp.ViewModel;
using System.Windows;

namespace Labb3QuizApp.Dialogs
{
    public partial class PackOptionsDialog : Window
    {
        public PackOptionsDialog()
        {
            InitializeComponent();
            //DataContext = new QuestionPackViewModel();
        }

        // PLACEHOLDER THIS WILL EDIT CURRENT PACK AND NOT CREATE

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (QuestionPackViewModel)DataContext;

            var editPack = new QuestionPack(viewModel.Name, viewModel.Difficulty, viewModel.TimeLimitInSeconds);

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}


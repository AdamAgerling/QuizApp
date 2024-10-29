using Labb3QuizApp.Model;
using Labb3QuizApp.ViewModel;
using System.Windows;

namespace Labb3QuizApp.Dialogs
{

    public partial class CreateNewPackDialog : Window
    {
        public CreateNewPackDialog()
        {
            InitializeComponent();
            DataContext = new CreateNewPackDialogViewModel();
        }
        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (CreateNewPackDialogViewModel)DataContext;

            var newPack = new QuestionPack(viewModel.PackName, viewModel.Difficulty, viewModel.TimeLimitInSeconds);

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


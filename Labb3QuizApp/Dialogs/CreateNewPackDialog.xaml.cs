using Labb3QuizApp.Model;
using Labb3QuizApp.Services;
using Labb3QuizApp.ViewModel;
using System.Windows;

namespace Labb3QuizApp.Dialogs
{
    public partial class CreateNewPackDialog : Window
    {
        private readonly MongoDbContext _dbContext;

        public CreateNewPackDialog(CategoryViewModel categoryViewModel, MongoDbContext dbContext, QuestionPackViewModel activePack)
        {
            InitializeComponent();
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            DataContext = new CreateNewPackDialogViewModel(categoryViewModel);
        }

        private async void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (CreateNewPackDialogViewModel)DataContext;

            if (string.IsNullOrWhiteSpace(viewModel.PackName))
            {
                MessageBox.Show("Pack name cannot be empty.");
                return;
            }

            var newPack = new QuestionPack(
                viewModel.PackName,
                viewModel.Difficulty,
                viewModel.TimeLimitInSeconds
            )
            {
                SelectedCategoryId = viewModel.SelectedCategoryId
            };

            try
            {
                if (_dbContext == null)
                {
                    MessageBox.Show("Database context is not initialized.");
                    return;
                }

                await _dbContext.AddQuestionPackAsync(newPack);
                MessageBox.Show("Question pack created successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to create question pack: {ex.Message}");
            }

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


using System.Diagnostics;
using System.Windows;

namespace Labb3QuizApp.Dialogs.CategoryDialog
{
    public partial class UpdateCategoryDialog : Window
    {
        public UpdateCategoryDialog(CategoryViewModel viewModel)
        {
            InitializeComponent();

            if (viewModel == null)
            {
                Debug.WriteLine("ViewModel passed to UpdateCategoryDialog is null.");
            }
            else
            {
                Debug.WriteLine($"ViewModel has {viewModel.Categories.Count} categories.");
            }

            DataContext = viewModel;

            if (viewModel.Categories.Any() && viewModel.SelectedCategory == null)
            {
                viewModel.SelectedCategory = viewModel.Categories.First();
            }
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not CategoryViewModel viewModel)
            {
                Debug.WriteLine("DataContext is null or not a CategoryViewModel.");
                return;
            }

            Debug.WriteLine($"Categories count: {viewModel.Categories.Count}");
            Debug.WriteLine($"SelectedCategory: {viewModel.SelectedCategory?.Name ?? "null"}");

            if (viewModel.SelectedCategory != null)
            {
                var newName = NewCategoryNameTextBox.Text;
                if (!string.IsNullOrWhiteSpace(newName))
                {
                    viewModel.SelectedCategory.Name = newName;
                    viewModel.UpdateCategory(viewModel.SelectedCategory);
                    MessageBox.Show($"The chosen category was updated to {newName} successfully!");
                    Close();
                }
                else
                {
                    Debug.WriteLine("UpdateButton_Click: New name is empty.");
                }
            }
            else
            {
                Debug.WriteLine("UpdateButton_Click: No category selected.");
            }
        }


        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}

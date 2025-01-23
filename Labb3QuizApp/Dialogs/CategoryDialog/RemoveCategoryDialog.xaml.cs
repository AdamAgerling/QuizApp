using System.Diagnostics;
using System.Windows;

namespace Labb3QuizApp.Dialogs.CategoryDialog
{

    public partial class RemoveCategoryDialog : Window
    {
        public RemoveCategoryDialog(CategoryViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;

            if (viewModel.Categories.Any())
            {
                viewModel.SelectedCategory = viewModel.Categories.First();
            }

            if (viewModel.SelectedCategory != null)
            {
                Debug.WriteLine($"Selected category: {viewModel.SelectedCategory.Name}");
            }
            else
            {
                Debug.WriteLine("No category selected.");
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is CategoryViewModel viewModel && viewModel.SelectedCategory.Id != null)
            {
                var categoryId = viewModel.SelectedCategory.Id.ToString();
                viewModel.RemoveCategory(categoryId);
                MessageBox.Show($"The category: {viewModel.SelectedCategory.Name} was removed successfully!");
                Close();
            }
            else
            {
                Debug.WriteLine("RemoveButton_Click: No category selected or DataContext is null.");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}

using Labb3QuizApp.Model;
using System.Windows;

namespace Labb3QuizApp.Dialogs.CategoryDialog
{
    public partial class AddCategoryDialog : Window
    {
        public Category NewCategory { get; private set; }
        public AddCategoryDialog()
        {
            InitializeComponent();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            NewCategory = new Category(Name)
            {
                Name = CategoryName.Text
            };
            DialogResult = true;
            MessageBox.Show($"The category: {CategoryName.Text} was added successfully!");
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}

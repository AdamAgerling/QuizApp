using Labb3QuizApp.ViewModel;
using System.Windows;

namespace Labb3QuizApp
{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainWindowViewModel();
        }
    }
}
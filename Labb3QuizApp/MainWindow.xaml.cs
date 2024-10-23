using Labb3QuizApp.Model;
using Labb3QuizApp.ViewModel;
using System.Windows;

namespace Labb3QuizApp
{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var questionPack = new QuestionPackViewModel(new QuestionPack("My Question Paack"));

        }
    }
}
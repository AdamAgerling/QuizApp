using Labb3QuizApp.ViewModel;
using System.Windows;

namespace Labb3QuizApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow = new MainWindow();
            MainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (MainWindow?.DataContext is MainWindowViewModel mainViewModel)
            {
                mainViewModel.MenuViewModel.StoreLastActivePack();
            }
            base.OnExit(e);
        }
    }
}

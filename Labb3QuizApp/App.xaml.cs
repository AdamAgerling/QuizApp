using Labb3QuizApp.Services;
using Labb3QuizApp.ViewModel;
using System.Configuration;
using System.Windows;

namespace Labb3QuizApp
{
    public partial class App : Application
    {
        string connectionString = ConfigurationManager.AppSettings["MongoConnectionString"];
        string databaseName = ConfigurationManager.AppSettings["MongoDatabaseName"];

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow = new MainWindow();
            MainWindow.Show();
            var mongoDataService = new MongoDataService(connectionString, databaseName, "QuestionPacks");
            await mongoDataService.SetupDefaultCategories();
        }

        async protected override void OnExit(ExitEventArgs e)
        {
            if (MainWindow?.DataContext is MainWindowViewModel mainViewModel)
            {
                await mainViewModel.MenuViewModel.StoreLastActivePack();
            }
            base.OnExit(e);
        }
    }
}

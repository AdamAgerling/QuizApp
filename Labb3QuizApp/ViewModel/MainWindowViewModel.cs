using Labb3QuizApp.Command;
using Labb3QuizApp.Services;
using System.Configuration;
using System.Windows;

namespace Labb3QuizApp.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
        private QuestionPackViewModel? _activePack;
        private readonly MongoDataService _mongoDataService;

        public MenuViewModel MenuViewModel { get; }

        public ConfigurationViewModel ConfigurationViewModel { get; }
        public PlayerViewModel PlayerViewModel { get; set; }

        private Visibility _playerViewVisibility = Visibility.Hidden;
        public Visibility PlayerViewVisibility
        {
            get => _playerViewVisibility;
            set
            {
                if (_playerViewVisibility != value)
                {
                    _playerViewVisibility = value;
                    RaisePropertyChanged(nameof(PlayerViewVisibility));
                }
            }
        }

        private Visibility _configurationVisibility = Visibility.Visible;
        public Visibility ConfigurationVisibility
        {
            get => _configurationVisibility;
            set
            {
                if (_configurationVisibility != value)
                {
                    _configurationVisibility = value;
                    RaisePropertyChanged(nameof(ConfigurationVisibility));
                }
            }
        }

        public DelegateCommand ShowPlayerView { get; }
        public DelegateCommand ShowConfigurationView { get; }
        public DelegateCommand ExitEnviroment { get; }
        public DelegateCommand ToggleFullScreen { get; set; }


        public QuestionPackViewModel? ActivePack
        {
            get => _activePack;
            set
            {
                _activePack = value;
                RaisePropertyChanged(nameof(ActivePack));
                ConfigurationViewModel?.RaisePropertyChanged();
                if (_activePack != null)
                {
                    RaisePropertyChanged(nameof(ActivePack.Questions));
                }
            }
        }

        public MainWindowViewModel()
        {
            string connectionString = ConfigurationManager.AppSettings["MongoConnectionString"];
            string databaseName = ConfigurationManager.AppSettings["MongoDatabaseName"];

            _mongoDataService = new MongoDataService(connectionString, databaseName);

            MenuViewModel = new MenuViewModel(this, _mongoDataService);
            Application.Current.Exit += (s, e) => MenuViewModel.StoreLastActivePack();

            ConfigurationViewModel = new ConfigurationViewModel(this, MenuViewModel, _mongoDataService);
            PlayerViewModel = new PlayerViewModel(this, MenuViewModel);

            ShowConfigurationView = new DelegateCommand(ShowConfigurationViewHandler);
            ShowPlayerView = new DelegateCommand(ShowPlayerViewHandler);
            ExitEnviroment = new DelegateCommand(ExitEnivromentHandler);
            ToggleFullScreen = new DelegateCommand(ToggleFullScreenHandler);
        }

        private void ShowPlayerViewHandler(object? obj)
        {
            if (ActivePack == null || ActivePack.Questions.Count == 0)
            {
                return;
            }

            PlayerViewVisibility = Visibility.Visible;
            ConfigurationVisibility = Visibility.Hidden;
            PlayerViewModel.StartQuiz(ActivePack.Questions, ActivePack.TimeLimitInSeconds);
        }

        private void ShowConfigurationViewHandler(object? obj)
        {
            ConfigurationVisibility = Visibility.Visible;
            PlayerViewVisibility = Visibility.Hidden;
            PlayerViewModel.StopQuiz();
        }

        private void ExitEnivromentHandler(object? obj)
        {
            Application.Current.Shutdown();
        }

        private void ToggleFullScreenHandler(object? obj)
        {
            var mainWindow = Application.Current.MainWindow;


            if (mainWindow.WindowState == WindowState.Normal)
            {
                mainWindow.WindowStyle = WindowStyle.None;
                mainWindow.WindowState = WindowState.Maximized;
            }
            else
            {
                mainWindow.WindowStyle = WindowStyle.SingleBorderWindow;
                mainWindow.WindowState = WindowState.Normal;
            }
        }
    }
}
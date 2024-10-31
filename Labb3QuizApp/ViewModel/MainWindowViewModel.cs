using Labb3QuizApp.Command;
using Labb3QuizApp.Model;
using Labb3QuizApp.Services;
using System.Windows;

namespace Labb3QuizApp.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
        private QuestionPackViewModel? _activePack;

        public MenuViewModel MenuViewModel { get; }
        public LocalDataService LocalDataService { get; }

        public ConfigurationViewModel ConfigurationViewModel { get; }
        public PlayerViewModel PlayerViewModel { get; }

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

        public QuestionPackViewModel? ActivePack
        {
            get => _activePack;
            set
            {
                _activePack = value;
                RaisePropertyChanged(nameof(ActivePack));
                if (_activePack != null)
                {
                    RaisePropertyChanged(nameof(ActivePack.Questions));
                }
            }
        }

        public MainWindowViewModel()
        {
            MenuViewModel = new MenuViewModel(this);
            ActivePack = new QuestionPackViewModel(new QuestionPack("My Question Pack"));
            PlayerViewModel = new PlayerViewModel(this);
            ConfigurationViewModel = new ConfigurationViewModel(this, MenuViewModel, LocalDataService);

            ShowConfigurationView = new DelegateCommand(ShowConfigurationViewHandler);
            ShowPlayerView = new DelegateCommand(ShowPlayerViewHandler);
        }

        private void ShowPlayerViewHandler(object? obj)
        {
            PlayerViewVisibility = Visibility.Visible;
            ConfigurationVisibility = Visibility.Hidden;
        }

        private void ShowConfigurationViewHandler(object? obj)
        {
            ConfigurationVisibility = Visibility.Visible;
            PlayerViewVisibility = Visibility.Hidden;
        }
    }
}

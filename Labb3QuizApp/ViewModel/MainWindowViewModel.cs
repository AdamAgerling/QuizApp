using Labb3QuizApp.Command;
using Labb3QuizApp.Model;
using Labb3QuizApp.Services;
using System.Collections.ObjectModel;

namespace Labb3QuizApp.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
        private QuestionPackViewModel? _activePack;

        public ObservableCollection<QuestionPackViewModel> QuestionPacks { get; set; }

        public DelegateCommand SelectPackCommand { get; }

        public PlayerViewModel PlayerViewModel { get; }
        public MenuViewModel MenuViewModel { get; }
        public LocalDataService LocalDataService { get; }

        public ConfigurationViewModel ConfigurationViewModel { get; }

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
            ConfigurationViewModel = new ConfigurationViewModel(this, MenuViewModel, LocalDataService);
            PlayerViewModel = new PlayerViewModel(this);
        }
    }
}

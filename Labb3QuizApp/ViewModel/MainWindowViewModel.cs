using Labb3QuizApp.Model;
using System.Collections.ObjectModel;

namespace Labb3QuizApp.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
        private QuestionPackViewModel? _activePack;

        public ObservableCollection<QuestionPackViewModel> QuestionPacks { get; set; }

        public PlayerViewModel PlayerViewModel { get; }
        public MenuViewModel MenuViewModel { get; }

        public ConfigurationViewModel ConfigurationViewModel { get; }

        public QuestionPackViewModel? ActivePack
        {
            get => _activePack;
            set
            {
                _activePack = value;
                RaisePropertyChanged();
                if (_activePack != null)
                {
                    RaisePropertyChanged(nameof(ActivePack.Questions));
                }
            }
        }
        public MainWindowViewModel()
        {
            MenuViewModel = new MenuViewModel(this);
            ConfigurationViewModel = new ConfigurationViewModel(this, MenuViewModel);
            PlayerViewModel = new PlayerViewModel(this);
            ActivePack = new QuestionPackViewModel(new QuestionPack("My Question Pack"));
        }

    }
}

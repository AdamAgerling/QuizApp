using Labb3QuizApp.Model;
using System.Collections.ObjectModel;

namespace Labb3QuizApp.ViewModel
{
    class CreateNewPackDialogViewModel : ViewModelBase
    {
        private string _packName;
        private Difficulty _difficulty;
        private int _timeLimitInSeconds;

        public string PackName
        {
            get => _packName;
            set
            {
                if (_packName != value)
                {
                    _packName = value;
                    RaisePropertyChanged(nameof(PackName));
                }
            }
        }

        public Difficulty Difficulty
        {
            get => _difficulty;
            set
            {
                if (_difficulty != value)
                {
                    _difficulty = value;
                    RaisePropertyChanged(nameof(Difficulty));
                }
            }
        }

        public int TimeLimitInSeconds
        {
            get => _timeLimitInSeconds;
            set
            {
                if (_timeLimitInSeconds != value)
                {
                    _timeLimitInSeconds = value;
                    RaisePropertyChanged(nameof(TimeLimitInSeconds));
                }
            }
        }
        public ObservableCollection<Difficulty> SelectDifficulty { get; }

        public CreateNewPackDialogViewModel()
        {
            PackName = "New Pack";
            SelectDifficulty = new ObservableCollection<Difficulty>(Enum.GetValues(typeof(Difficulty)).Cast<Difficulty>());
            TimeLimitInSeconds = 30;
        }

    }
}


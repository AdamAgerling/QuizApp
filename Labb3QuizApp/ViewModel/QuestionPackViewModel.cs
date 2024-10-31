using Labb3QuizApp.Model;
using System.Collections.ObjectModel;

namespace Labb3QuizApp.ViewModel
{
    class QuestionPackViewModel : ViewModelBase
    {
        private readonly QuestionPack _model;
        public QuestionPack QuestionPack => _model;

        public ObservableCollection<Question> Questions { get; }

        public string Name
        {
            get => _model.Name;
            set
            {
                _model.Name = value;
                RaisePropertyChanged(nameof(Name));
            }
        }
        public Difficulty Difficulty
        {
            get => _model.Difficulty;
            set
            {
                _model.Difficulty = value;
                RaisePropertyChanged(nameof(Difficulty));
            }
        }
        public int TimeLimitInSeconds
        {
            get => _model.TimeLimitInSeconds;
            set
            {
                _model.TimeLimitInSeconds = value;
                RaisePropertyChanged(nameof(TimeLimitInSeconds));
            }
        }

        public QuestionPackViewModel(QuestionPack model)
        {
            _model = model;
            Questions = new ObservableCollection<Question>(model.Questions);
        }
        public override string ToString()
        {
            return $"{Name} ({Difficulty})";
        }
    }
}

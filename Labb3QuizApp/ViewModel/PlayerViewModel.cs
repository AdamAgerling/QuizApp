using Labb3QuizApp.Model;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace Labb3QuizApp.ViewModel
{
    class PlayerViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel? mainWindowViewModel;
        private readonly Random _random = new Random();
        private DispatcherTimer _timer;
        private ObservableCollection<Question> _randomizedQuestions;
        private Question? _currentQuestion;

        public ObservableCollection<Question> RandomizedQuestions
        {
            get => _randomizedQuestions;
            set
            {
                _randomizedQuestions = value;
                RaisePropertyChanged(nameof(RandomizedQuestions));
            }
        }

        public Question? CurrentQuestion
        {
            get => _currentQuestion;
            set
            {
                _currentQuestion = value;
                RaisePropertyChanged(nameof(CurrentQuestion));
            }
        }


        public PlayerViewModel(MainWindowViewModel? mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel;

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += QuestionTimer;
            _timer.Start();

            //UpdateButtonCommand = new DelegateCommand(UpdateButton, CanUpdateButton);

        }

        private void QuestionTimer(object? sender, EventArgs e)
        {
            //_timer += 
        }

        public void StartQuiz(ObservableCollection<Question> question)
        {

        }

    }
}

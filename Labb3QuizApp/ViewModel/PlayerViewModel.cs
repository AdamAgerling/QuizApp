using Labb3QuizApp.Command;
using Labb3QuizApp.Model;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Threading;

namespace Labb3QuizApp.ViewModel
{
    class PlayerViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel? _mainWindowViewModel;
        private readonly Random _random = new Random();
        private DispatcherTimer _timer;
        private ObservableCollection<Question> _randomizedQuestions;
        private Question _currentQuestion;
        private string[] _currentAnswers;
        public ObservableCollection<Question> RandomizedQuestions
        {
            get => _randomizedQuestions;
            set
            {
                _randomizedQuestions = value;
                RaisePropertyChanged(nameof(RandomizedQuestions));
            }
        }

        public Question CurrentQuestion
        {
            get => _currentQuestion;
            set
            {
                _currentQuestion = value;
                RaisePropertyChanged(nameof(CurrentQuestion));
                _currentAnswers = GetShuffledAnswers(CurrentQuestion);
                RaisePropertyChanged(nameof(Answer1));
                RaisePropertyChanged(nameof(Answer2));
                RaisePropertyChanged(nameof(Answer3));
                RaisePropertyChanged(nameof(Answer4));
            }
        }

        public string Answer1 => _currentAnswers?.Length > 0 ? _currentAnswers[0] : string.Empty;
        public string Answer2 => _currentAnswers?.Length > 1 ? _currentAnswers[1] : string.Empty;
        public string Answer3 => _currentAnswers?.Length > 2 ? _currentAnswers[2] : string.Empty;
        public string Answer4 => _currentAnswers?.Length > 3 ? _currentAnswers[3] : string.Empty;

        public DelegateCommand SelectedAnswer { get; }


        public PlayerViewModel(MainWindowViewModel? mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
            _randomizedQuestions = new ObservableCollection<Question>();

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += QuestionTimer;

            SelectedAnswer = new DelegateCommand(param =>
            {
                if (param is string selectedAnswer)
                    OnSelectedAnswer(selectedAnswer);
            });
        }

        private void OnSelectedAnswer(string selectedAnswer)
        {
            if (selectedAnswer == CurrentQuestion.CorrectAnswer)
            {
                LoadNextQuestion();
            }
            else
            {
                LoadNextQuestion();
            }
        }

        private void QuestionTimer(object? sender, EventArgs e)
        {
            LoadNextQuestion();
        }

        public void StartQuiz(ObservableCollection<Question> questions)
        {
            if (RandomizedQuestions == null || questions.Count == 0)
            {
                return;
            }
            RandomizedQuestions = new ObservableCollection<Question>(questions.OrderBy(q => _random.Next()).ToList());


            LoadNextQuestion();
            _timer.Start();
        }

        private void LoadNextQuestion()
        {
            if (RandomizedQuestions.Count > 0)
            {
                CurrentQuestion = RandomizedQuestions[0];
                RandomizedQuestions.RemoveAt(0);
            }
            else
            {
                _timer.Stop();
            }
        }
        private string[] GetShuffledAnswers(Question question)
        {
            var answers = question.InCorrectAnswers.Append(question.CorrectAnswer).ToList();
            return answers.OrderBy(a => _random.Next()).ToArray();
        }

        public void StopQuiz()
        {
            _timer?.Stop();
            RandomizedQuestions.Clear();
            Debug.WriteLine("Quiz stopped.");
        }
    }
}

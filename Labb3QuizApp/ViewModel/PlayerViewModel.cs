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
        private int _currentQuestionCount;
        private int _totalQuestions;
        private int _questionScore;
        private int _timePerQuestion;
        private bool _isCorrectAnswer;
        private string _selectedAnswer;
        private bool _isAnswered;
        private bool _isQuizActive;

        public bool IsQuizActive
        {
            get => _isQuizActive;
            set
            {
                if (_isQuizActive != value)
                {
                    _isQuizActive = value;
                    RaisePropertyChanged(nameof(IsQuizActive));
                    RaisePropertyChanged(nameof(IsGameOver));
                }
            }
        }

        public bool IsGameOver => !IsQuizActive;

        public bool IsAnswered
        {
            get => _isAnswered;
            set
            {
                if (_isAnswered != value)
                {
                    _isAnswered = value;
                    RaisePropertyChanged(nameof(IsAnswered));
                }
            }
        }
        public bool IsCorrectAnswer
        {
            get => _isCorrectAnswer;
            set
            {
                _isCorrectAnswer = value;
                RaisePropertyChanged(nameof(IsCorrectAnswer));
            }
        }

        public int QuestionScore
        {
            get => _questionScore;
            set
            {
                _questionScore = value;
                RaisePropertyChanged(nameof(QuestionScore));
            }
        }

        public int TimePerQuestion
        {
            get => _timePerQuestion;
            set
            {
                _timePerQuestion = value;
                RaisePropertyChanged(nameof(TimePerQuestion));
            }
        }

        public int CurrentQuestionCount
        {
            get => _currentQuestionCount;
            set
            {
                _currentQuestionCount = value;
                RaisePropertyChanged(nameof(CurrentQuestionCount));
                RaisePropertyChanged(nameof(QuestionProgress));
            }
        }

        public int TotalQuestions
        {
            get => _totalQuestions;
            set
            {
                _totalQuestions = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(QuestionProgress));
            }
        }

        public string SelectedAnswer
        {
            get => _selectedAnswer;
            set
            {
                if (_selectedAnswer != value)
                {
                    _selectedAnswer = value;
                    RaisePropertyChanged(nameof(SelectedAnswer));
                }
            }
        }

        public ObservableCollection<Question> RandomizedQuestions
        {
            get => _randomizedQuestions;
            set
            {
                _randomizedQuestions = value;
                RaisePropertyChanged(nameof(RandomizedQuestions));
            }
        }
        public string QuestionProgress => $"Question {CurrentQuestionCount} out of {TotalQuestions}";
        public string ScoreText => $"You got {QuestionScore} out of {TotalQuestions} possible!";

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

        public DelegateCommand SelectAnswer { get; }

        public PlayerViewModel(MainWindowViewModel? mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
            _randomizedQuestions = new ObservableCollection<Question>();

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += QuestionTimer;

            SelectAnswer = new DelegateCommand(param =>
            {
                if (param is string selectedAnswer)
                    OnSelectedAnswer(selectedAnswer);
            });
        }

        private void OnSelectedAnswer(string selectedAnswer)
        {
            if (!IsAnswered)
            {
                SelectedAnswer = selectedAnswer;
                IsAnswered = true;
                IsCorrectAnswer = selectedAnswer == CurrentQuestion.CorrectAnswer;
            }
            if (IsCorrectAnswer)
            {
                QuestionScore++;

            }
            Task.Delay(2000).ContinueWith(_ => LoadNextQuestion());
        }

        private void QuestionTimer(object? sender, EventArgs e)
        {
            if (TimePerQuestion > 0)
            {
                TimePerQuestion--;
            }
            else
            {
                IsAnswered = true;
                IsCorrectAnswer = false;
                LoadNextQuestion();
                TimePerQuestion = _mainWindowViewModel.ActivePack.TimeLimitInSeconds;
            }
        }

        public void StartQuiz(ObservableCollection<Question> questions, int timeLimitInSeconds)
        {
            IsQuizActive = true;
            Debug.WriteLine($"IsQuizActive: {IsQuizActive}");
            if (RandomizedQuestions == null || questions.Count == 0)
            {
                return;
            }
            RandomizedQuestions = new ObservableCollection<Question>(questions.OrderBy(q => _random.Next()).ToList());

            if (RandomizedQuestions.Any())
            {
                TotalQuestions = RandomizedQuestions.Count;
                CurrentQuestionCount = 1;
                RaisePropertyChanged(nameof(QuestionProgress));
            }

            TimePerQuestion = _mainWindowViewModel.ActivePack.TimeLimitInSeconds;
            LoadNextQuestion();
            _timer.Start();
        }

        private void LoadNextQuestion()
        {
            SelectedAnswer = null;
            IsAnswered = false;

            if (RandomizedQuestions.Count > 0)
            {
                CurrentQuestion = RandomizedQuestions[0];
                RandomizedQuestions.RemoveAt(0);
                CurrentQuestionCount = TotalQuestions - RandomizedQuestions.Count;

                TimePerQuestion = _mainWindowViewModel.ActivePack.TimeLimitInSeconds;
                RaisePropertyChanged(nameof(ScoreText));

            }
            else
            {
                _timer.Stop();
                IsQuizActive = false;
                RaisePropertyChanged(nameof(ScoreText));
                Debug.WriteLine("No more questions left. Quiz ended.");
            }
        }


        private string[] GetShuffledAnswers(Question question)
        {
            var answers = question.InCorrectAnswers.Append(question.CorrectAnswer).ToList();
            return answers.OrderBy(a => _random.Next()).ToArray();
        }

        public void StopQuiz()
        {
            IsQuizActive = false;
            Debug.WriteLine($"IsQuizActive: {IsQuizActive}");
            QuestionScore = 0;
            _timer.Stop();
            RandomizedQuestions.Clear();
        }
    }
}

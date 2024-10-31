using Labb3QuizApp.Command;
using Labb3QuizApp.Model;
using Labb3QuizApp.Services;

namespace Labb3QuizApp.ViewModel
{
    class ConfigurationViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel? _mainWindowViewModel;
        private readonly LocalDataService? _localDataService;

        public MenuViewModel? MenuViewModel { get; }
        public DelegateCommand RemoveQuestion { get; }
        public DelegateCommand AddQuestion { get; }

        private Question? _selectedQuestion;

        public Question? SelectedQuestion
        {
            get => _selectedQuestion;
            set
            {
                if (_selectedQuestion != value)
                {
                    _selectedQuestion = value;
                    RaisePropertyChanged();
                    RemoveQuestion.RaiseCanExecuteChanged();
                }
            }
        }
        public QuestionPackViewModel? ActivePack { get => _mainWindowViewModel?.ActivePack; }

        public ConfigurationViewModel(MainWindowViewModel? mainWindowViewModel, MenuViewModel? menuViewModel, LocalDataService? localDataService)
        {
            MenuViewModel = menuViewModel;
            _mainWindowViewModel = mainWindowViewModel;
            _localDataService = localDataService ?? new LocalDataService();

            var loadedQuestionPacks = _localDataService?.LoadQuestionPacks();

            if (loadedQuestionPacks != null)
            {
                foreach (var pack in loadedQuestionPacks)
                {
                    if (pack.Name == ActivePack?.Name)
                    {
                        ActivePack.Questions.Clear();
                        foreach (var question in pack.Questions)
                        {
                            ActivePack?.Questions.Add(question);
                        }
                    }
                }
            }

            AddQuestion = new DelegateCommand(AddQuestionHandler);
            RemoveQuestion = new DelegateCommand(RemoveQuestionHandler, CanRemoveQuestion);
        }
        private void RemoveQuestionHandler(object? obj)
        {
            if (SelectedQuestion != null)
            {
                ActivePack?.Questions.Remove(SelectedQuestion);
                SelectedQuestion = null;
                _localDataService?.SaveQuestions(ActivePack?.Questions.ToList(), ActivePack?.Name);
            }
        }
        private bool CanRemoveQuestion(object? obj) => SelectedQuestion != null;

        private void AddQuestionHandler(object? obj)
        {
            var newQuestion = new Question("New Question", "", "", "", "");
            ActivePack?.Questions.Add(newQuestion);
            SelectedQuestion = newQuestion;
            _localDataService?.SaveQuestions(ActivePack?.Questions.ToList(), ActivePack?.Name);
        }
        public void UpdateQuestion()
        {
            if (ActivePack?.Questions != null)
            {
                _localDataService?.SaveQuestions(ActivePack?.Questions.ToList(), ActivePack?.Name);
            }
        }
    }
}

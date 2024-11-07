using Labb3QuizApp.Command;
using Labb3QuizApp.Model;
using Labb3QuizApp.Services;
namespace Labb3QuizApp.ViewModel
{
    class ConfigurationViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel? _mainWindowViewModel;
        private readonly LocalDataService? _localDataService;

        public List<Difficulty> SelectDifficulty { get; } = Enum.GetValues(typeof(Difficulty)).Cast<Difficulty>().ToList();

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
        public QuestionPackViewModel? ActivePack
        {
            get => _mainWindowViewModel?.ActivePack;
            set
            {
                if (_mainWindowViewModel != null && _mainWindowViewModel.ActivePack != value)
                {
                    _mainWindowViewModel.ActivePack = value;
                    RaisePropertyChanged(nameof(ActivePack));
                }
            }
        }

        public ConfigurationViewModel(MainWindowViewModel? mainWindowViewModel, MenuViewModel? menuViewModel, LocalDataService? localDataService)
        {
            MenuViewModel = menuViewModel;
            _mainWindowViewModel = mainWindowViewModel;
            _localDataService = localDataService ?? new LocalDataService(menuViewModel);

            InitializeDataAsync();

            AddQuestion = new DelegateCommand(AddQuestionHandler);
            RemoveQuestion = new DelegateCommand(RemoveQuestionHandler, CanRemoveQuestion);
        }

        private async void InitializeDataAsync()
        {
            var loadedQuestionPacks = await _localDataService.LoadQuestionPacks();

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
        }

        private async void RemoveQuestionHandler(object? obj)
        {
            if (SelectedQuestion != null)
            {
                ActivePack?.Questions.Remove(SelectedQuestion);
                SelectedQuestion = null;
                if (_localDataService != null && ActivePack?.Questions != null && ActivePack.Name != null)
                {
                    await _localDataService.SaveQuestions(ActivePack.Questions.ToList(), ActivePack.Name);
                }
            }
        }
        private bool CanRemoveQuestion(object? obj) => SelectedQuestion != null;

        private async void AddQuestionHandler(object? obj)
        {
            var newQuestion = new Question("New Question", "", "", "", "");
            ActivePack?.Questions.Add(newQuestion);
            SelectedQuestion = newQuestion;
            RaisePropertyChanged(nameof(ActivePack.Questions));
            if (_localDataService != null && ActivePack?.Questions != null && ActivePack.Name != null)
            {
                await _localDataService.SaveQuestions(ActivePack.Questions.ToList(), ActivePack.Name);
            }
        }

        public async Task UpdateQuestion()
        {
            if (ActivePack?.Questions != null && ActivePack.Name != null)
            {
                await _localDataService.SaveQuestions(ActivePack.Questions.ToList(), ActivePack.Name);
            }
        }

        public async Task UpdatePack()
        {
            if (ActivePack == null || ActivePack.Name == null)
            {
                return;
            }

            var packs = await _localDataService.LoadQuestionPacks();

            if (packs == null)
            {
                return;
            }

            var packToUpdate = packs.FirstOrDefault(p => p.Name == ActivePack.Name);
            if (packToUpdate != null)
            {
                packToUpdate.Name = ActivePack.Name;
                packToUpdate.Difficulty = ActivePack.Difficulty;
                packToUpdate.TimeLimitInSeconds = ActivePack.TimeLimitInSeconds;

                RaisePropertyChanged(nameof(ActivePack));
                await _localDataService.SaveQuestionPacks(packs);
            }
        }
    }
}
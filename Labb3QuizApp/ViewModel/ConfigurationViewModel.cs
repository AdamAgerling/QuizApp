using Labb3QuizApp.Command;
using Labb3QuizApp.Model;
using Labb3QuizApp.Services;
using MongoDB.Driver;
using System.Diagnostics;

namespace Labb3QuizApp.ViewModel
{
    class ConfigurationViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel? _mainWindowViewModel;
        private readonly MongoDataService _mongoDataService;

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

        public ConfigurationViewModel(MainWindowViewModel? mainWindowViewModel, MenuViewModel? menuViewModel, MongoDataService? mongoDataService)
        {
            MenuViewModel = menuViewModel;
            _mainWindowViewModel = mainWindowViewModel;
            _mongoDataService = mongoDataService;

            LoadDataAsync();

            AddQuestion = new DelegateCommand(AddQuestionHandler);
            RemoveQuestion = new DelegateCommand(RemoveQuestionHandler, CanRemoveQuestion);
        }

        public async Task InitializeAsync()
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            var loadedQuestionPacks = await _mongoDataService.LoadQuestionPacks();

            if (loadedQuestionPacks != null)
            {
                Debug.WriteLine("Loaded question packs from MongoDB.");
                foreach (var pack in loadedQuestionPacks)
                {
                    Debug.WriteLine($"ActivePack before loading data: {ActivePack?.Name}, Questions count: {ActivePack?.Questions.Count}");

                    if (pack.Name == ActivePack?.Name && pack.Questions != null)
                    {
                        Debug.WriteLine($"Found active pack: {pack.Name}, Questions count in database: {pack.Questions.Count}");

                        ActivePack.Questions.Clear();
                        foreach (var question in pack.Questions)
                        {
                            Debug.WriteLine($"Adding question: {question.QuizQuestion}");

                            ActivePack?.Questions.Add(question);
                        }
                    }
                    Debug.WriteLine($"ActivePack after loading data: {ActivePack?.Name}, Questions count: {ActivePack?.Questions.Count}");

                    if (ActivePack == null || string.IsNullOrWhiteSpace(ActivePack.Name))
                    {
                        Debug.WriteLine("ActivePack is null or has an invalid name. Skipping data load.");
                        return;
                    }
                }
            }
        }

        private async void RemoveQuestionHandler(object? obj)
        {
            Debug.WriteLine("RemoveQuestionHandler is being called.");
            if (SelectedQuestion != null)
            {
                ActivePack?.Questions.Remove(SelectedQuestion);
                SelectedQuestion = null;
                if (_mongoDataService != null && ActivePack?.Questions != null && ActivePack.Name != null)
                {
                    Debug.WriteLine($"Saving question pack after removing question: {ActivePack.Name}");
                    await _mongoDataService.SaveQuestions(ActivePack.Questions.ToList(), ActivePack.Name);
                }
            }
        }

        private bool CanRemoveQuestion(object? obj) => SelectedQuestion != null;

        private async void AddQuestionHandler(object? obj)
        {
            Debug.WriteLine("AddQuestionHandler is being called.");
            var newQuestion = new Question("New Question", "", "", "", "");
            ActivePack?.Questions.Add(newQuestion);
            SelectedQuestion = newQuestion;
            RaisePropertyChanged(nameof(ActivePack.Questions));
            if (_mongoDataService != null && ActivePack?.Questions != null && ActivePack.Name != null)
            {
                Debug.WriteLine($"Saving question pack: {ActivePack.Name}");
                await _mongoDataService.SaveQuestions(ActivePack.Questions.ToList(), ActivePack.Name);
            }
        }

        public async Task UpdateQuestion()
        {
            if (ActivePack?.Questions != null && ActivePack.Name != null)
            {
                await _mongoDataService.SaveQuestions(ActivePack.Questions.ToList(), ActivePack.Name);
            }
        }

        public async Task UpdatePack()
        {
            if (ActivePack == null || string.IsNullOrEmpty(ActivePack.Name))
            {
                Debug.WriteLine("ActivePack is null or has an empty name.");
                return;
            }

            ActivePack.QuestionPack.SelectedCategoryId = ActivePack.SelectedCategoryId;

            await SaveActivePack();

            Debug.WriteLine($"Pack '{ActivePack.Name}' updated successfully.");
        }

        private async Task SaveActivePack()
        {
            if (ActivePack == null || ActivePack.QuestionPack == null)
            {
                Debug.WriteLine("ActivePack or its QuestionPack is null.");
                return;
            }

            var collection = _mongoDataService._dbContext.QuestionPacks;
            var filter = Builders<QuestionPack>.Filter.Eq(p => p.Id, ActivePack.QuestionPack.Id);

            var update = Builders<QuestionPack>.Update
                .Set(p => p.Name, ActivePack.Name)
                .Set(p => p.Difficulty, ActivePack.Difficulty)
                .Set(p => p.TimeLimitInSeconds, ActivePack.TimeLimitInSeconds)
                .Set(p => p.SelectedCategoryId, ActivePack.SelectedCategoryId)
                .Set(p => p.Questions, ActivePack.Questions.ToList());

            var result = await collection.UpdateOneAsync(filter, update);

            if (result.ModifiedCount > 0)
            {
                Debug.WriteLine($"Successfully updated pack: {ActivePack.Name}");
            }
            else
            {
                Debug.WriteLine($"No matching pack found to update: {ActivePack.Name}");
            }
        }
    }
}
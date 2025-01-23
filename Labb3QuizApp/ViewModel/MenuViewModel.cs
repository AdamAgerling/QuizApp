using Labb3QuizApp.Command;
using Labb3QuizApp.Dialogs;
using Labb3QuizApp.Dialogs.CategoryDialog;
using Labb3QuizApp.Model;
using Labb3QuizApp.Services;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Labb3QuizApp.ViewModel
{
    class MenuViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel? _mainWindowViewModel;
        private CategoryViewModel? _categoryViewModel;
        private QuestionPackViewModel? _activePack;
        public ObservableCollection<Category> Categories { get; set; } = new ObservableCollection<Category>();

        string connectionString = ConfigurationManager.AppSettings["MongoConnectionString"];
        string databaseName = ConfigurationManager.AppSettings["MongoDatabaseName"];
        private readonly MongoDataService _mongoDataService;
        private readonly MongoDbContext _dbContext;
        private bool _isPlayMode;
        private bool _hasQuestions;
        private readonly string appDataFolder;
        private readonly string lastActivePackPath;
        public bool IsEditMode => !IsPlayMode;


        public DelegateCommand NavigateToQuiz { get; }
        public DelegateCommand NavigateToConfiguration { get; }
        public DelegateCommand OpenCreateNewPack { get; }
        public DelegateCommand OpenPackOptions { get; }
        public DelegateCommand SelectQuestionPack { get; }
        public DelegateCommand DeleteActivePack { get; }
        public DelegateCommand AddCategoryDialogCommand { get; }
        public DelegateCommand UpdateCategoryDialogCommand { get; }
        public DelegateCommand RemoveCategoryDialogCommand { get; }
        public ObservableCollection<QuestionPackViewModel> QuestionPacks { get; set; } = new ObservableCollection<QuestionPackViewModel>();

        public QuestionPackViewModel? ActivePack
        {
            get => _activePack;
            set
            {
                if (_activePack != value)
                {
                    if (_activePack?.Questions != null)
                    {
                        _activePack.Questions.CollectionChanged -= QuestionsCollectionChanged;
                    }
                    _activePack = value;
                    RaisePropertyChanged(nameof(ActivePack));


                    if (_activePack?.Questions != null)
                    {
                        _activePack.Questions.CollectionChanged += QuestionsCollectionChanged;
                    }
                    CheckHasQuestions();
                }
            }
        }

        private void QuestionsCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            CheckHasQuestions();
        }

        private void CheckHasQuestions()
        {
            HasQuestions = _activePack?.Questions.Count > 0;
        }

        public bool HasQuestions
        {
            get => _hasQuestions;
            set
            {
                if (_hasQuestions != value)
                {
                    _hasQuestions = value;
                    RaisePropertyChanged(nameof(HasQuestions));
                }
            }
        }

        public CategoryViewModel CategoryViewModel
        {
            get
            {
                if (_categoryViewModel == null)
                {
                    var mongoDataService = new MongoDataService(connectionString, databaseName, "Categories");
                    _categoryViewModel = new CategoryViewModel(mongoDataService);
                }
                return _categoryViewModel;
            }
        }
        public bool IsPlayMode
        {
            get => _isPlayMode;
            set
            {
                if (_isPlayMode != value)
                {
                    _isPlayMode = value;
                    RaisePropertyChanged(nameof(IsPlayMode));
                    RaisePropertyChanged(nameof(IsEditMode));
                }
            }
        }
        public MenuViewModel(MainWindowViewModel? mainWindowViewModel, MongoDataService mongoDataService, CategoryViewModel categoryViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
            _mongoDataService = mongoDataService;
            _categoryViewModel = new CategoryViewModel(mongoDataService);
            appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "QuizApp");
            Directory.CreateDirectory(appDataFolder);
            lastActivePackPath = Path.Combine(appDataFolder, "LastActivePack.txt");

            _dbContext = new MongoDbContext(connectionString, databaseName);
            LoadCategoriesAsync();
            OpenCreateNewPack = new DelegateCommand(CreateNewPackDialog);
            OpenPackOptions = new DelegateCommand(PackOptionsDialog);
            SelectQuestionPack = new DelegateCommand(SelectPack);
            DeleteActivePack = new DelegateCommand(RemoveActivePack);

            AddCategoryDialogCommand = new DelegateCommand(OpenAddCategoryDialog);
            UpdateCategoryDialogCommand = new DelegateCommand(OpenUpdateCategoryDialog);
            RemoveCategoryDialogCommand = new DelegateCommand(OpenRemoveCategoryDialog);


            NavigateToConfiguration = new DelegateCommand(obj =>
            {
                _mainWindowViewModel?.ShowConfigurationView.Execute(obj);
            });
            NavigateToQuiz = new DelegateCommand(obj =>
            {
                _mainWindowViewModel?.ShowPlayerView.Execute(obj);
            });
            _ = LoadAsync();

            if (ActivePack?.Questions != null)
            {
                ActivePack.Questions.CollectionChanged += (s, e) =>
                {
                    HasQuestions = ActivePack.Questions.Count > 0;
                };
            }
        }

        private void OpenAddCategoryDialog(object? obj)
        {
            AddCategoryDialog dialog = new();
            dialog.DataContext = new Category();

            if (dialog.ShowDialog() == true)
            {
                var newCategory = dialog.NewCategory;
                if (newCategory != null)
                {
                    _categoryViewModel?.AddCategory(newCategory);
                }
            }
        }

        private void OpenRemoveCategoryDialog(object? obj)
        {
            if (_categoryViewModel == null)
            {
                Debug.WriteLine("CategoryViewModel is null.");
                return;
            }

            var dialog = new RemoveCategoryDialog(_categoryViewModel);

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                MessageBox.Show("Category removed successfully.");
            }
        }

        private void OpenUpdateCategoryDialog(object? obj)
        {
            var dialog = new UpdateCategoryDialog(_categoryViewModel);

            if (dialog.ShowDialog() == true)
            {
                if (_categoryViewModel.SelectedCategory != null)
                {
                    _categoryViewModel.UpdateCategory(_categoryViewModel.SelectedCategory);
                }
                else
                {
                    Debug.WriteLine("No category selected after dialog was closed.");
                }
            }
            else
            {
                Debug.WriteLine("Dialog cancelled by user.");
            }
        }

        private async void CreateNewPackDialog(object? obj)
        {
            var dialog = new CreateNewPackDialog(CategoryViewModel, _dbContext, _activePack);

            if (dialog.ShowDialog() == true)
            {
                var viewModel = (CreateNewPackDialogViewModel)dialog.DataContext;
                var newPack = new QuestionPack(
                    viewModel.PackName,
                    viewModel.Difficulty,
                    viewModel.TimeLimitInSeconds
                );
                await SaveCurrentPacks();
                QuestionPacks.Add(new QuestionPackViewModel(newPack, Categories));
            }
        }

        private async Task LoadCategoriesAsync()
        {
            var categories = await _mongoDataService.GetCategories();
            Categories.Clear();
            foreach (var category in categories)
            {
                Categories.Add(category);
            }
        }

        private void SelectPack(object? obj)
        {
            if (obj is QuestionPackViewModel selectedPack)
            {
                ActivePack = selectedPack;
                _mainWindowViewModel.ActivePack = selectedPack;

                RaisePropertyChanged(nameof(ActivePack));
                RaisePropertyChanged(nameof(HasQuestions));
            }
            else
            {
                Debug.WriteLine("No valid pack selected.");
            }
        }

        private void RemoveActivePack(object? obj)
        {
            DialogResult dialogResult = MessageBox.Show("Do you really want to delete the current active pack? ",
                "Remove Pack", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                if (obj is QuestionPackViewModel selectedPack)
                {
                    if (QuestionPacks.Contains(selectedPack))
                    {
                        if (selectedPack.Name == "Default Pack")
                        {
                            DialogResult dialogNotPossible =
                                MessageBox.Show("You can't remove the Default Pack, you can only remove the questions inside it.",
                                "Not possible", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            QuestionPacks.Remove(selectedPack);
                            selectedPack.Questions.Clear();
                            _mongoDataService.RemoveQuestionPack(selectedPack.QuestionPack);
                        }

                        if (selectedPack == ActivePack)
                        {
                            ActivePack = QuestionPacks.FirstOrDefault(p => p.Name == "Default Pack") ?? CreateDefaultPack();
                            _mainWindowViewModel.ActivePack = ActivePack;
                        }

                        if (QuestionPacks.Count == 0 || !QuestionPacks.Any(p => p.Name == "Default Pack"))
                        {
                            var defaultPack = CreateDefaultPack();
                            QuestionPacks.Add(defaultPack);
                            ActivePack = defaultPack;
                            _mainWindowViewModel.ActivePack = ActivePack;
                        }
                    }
                }
            }
        }

        private QuestionPackViewModel CreateDefaultPack()
        {
            var defaultPack = new QuestionPack("Default Pack", Difficulty.Easy, 30);
            return new QuestionPackViewModel(defaultPack, Categories);
        }
        private void PackOptionsDialog(object? obj)
        {
            PackOptionsDialog packOptionsDialog = new(_categoryViewModel, _activePack);

            var configurationViewModel = new ConfigurationViewModel(_mainWindowViewModel, this, _mongoDataService);
            packOptionsDialog.DataContext = configurationViewModel;

            packOptionsDialog.ShowDialog();
        }

        private async Task LoadQuestionPacks()
        {
            var dataService = new MongoDataService(connectionString, databaseName, "QuestionPacks");

            var packs = await dataService.LoadQuestionPacks();

            foreach (var pack in packs)
            {
                var packViewModel = new QuestionPackViewModel(pack, Categories);
                QuestionPacks.Add(packViewModel);
            }

            if (ActivePack == null && QuestionPacks.Count > 0)
            {
                ActivePack = QuestionPacks.First();
                _mainWindowViewModel.ActivePack = ActivePack;
            }
        }

        public async Task SaveCurrentPacks()
        {
            Debug.WriteLine("Starting SaveCurrentPacks...");

            var dataService = new MongoDataService(connectionString, databaseName, "QuestionPacks");
            var existingPacks = await dataService.LoadQuestionPacks();

            if (existingPacks == null)
            {
                Debug.WriteLine("No existing packs found. Initializing a new list.");
                existingPacks = new List<QuestionPack>();
            }

            var updatedPacks = QuestionPacks.Select(p => p.QuestionPack).ToList();

            foreach (var updatedPack in updatedPacks)
            {
                var existingPack = existingPacks.FirstOrDefault(p => p.Name == updatedPack.Name);

                if (existingPack != null)
                {
                    Debug.WriteLine($"Updating existing pack with Name: {existingPack.Name}");
                    existingPacks.Remove(existingPack);
                }
                else
                {
                    Debug.WriteLine($"Adding new pack: {updatedPack.Name}");
                }

                existingPacks.Add(updatedPack);

                Debug.WriteLine($"Pack Name: {updatedPack.Name}, Question count: {updatedPack.Questions.Count}");
            }

            await dataService.SaveMultipleQuestionPacks(existingPacks);
            Debug.WriteLine("Finished saving packs.");
        }

        public async Task StoreLastActivePack()
        {
            if (ActivePack?.Name != null)
            {
                await File.WriteAllTextAsync(lastActivePackPath, ActivePack.Name);
            }
        }
        private async Task LoadLastActivePack()
        {
            if (File.Exists(lastActivePackPath))
            {
                var lastPackName = await File.ReadAllTextAsync(lastActivePackPath);
                var pack = QuestionPacks.FirstOrDefault(p => p.Name == lastPackName);
                if (pack != null)
                {
                    ActivePack = pack;
                    _mainWindowViewModel.ActivePack = pack;
                }
            }
        }
        private async Task LoadAsync()
        {
            await LoadQuestionPacks();
            await LoadLastActivePack();
        }
    }
}

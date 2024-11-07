using Labb3QuizApp.Command;
using Labb3QuizApp.Dialogs;
using Labb3QuizApp.Model;
using Labb3QuizApp.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Labb3QuizApp.ViewModel
{
    class MenuViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel? _mainWindowViewModel;
        private QuestionPackViewModel? _activePack;
        private LocalDataService _localDataService;
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
        public MenuViewModel(MainWindowViewModel? mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
            _localDataService = new LocalDataService(this);
            appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "QuizApp");
            Directory.CreateDirectory(appDataFolder);
            lastActivePackPath = Path.Combine(appDataFolder, "LastActivePack.txt");

            OpenCreateNewPack = new DelegateCommand(CreateNewPackDialog);
            OpenPackOptions = new DelegateCommand(PackOptionsDialog);
            SelectQuestionPack = new DelegateCommand(SelectPack);
            DeleteActivePack = new DelegateCommand(RemoveActivePack);


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

        private async void CreateNewPackDialog(object? obj)
        {
            CreateNewPackDialog createNewPackDialog = new();
            var viewModel = new CreateNewPackDialogViewModel();
            createNewPackDialog.DataContext = viewModel;

            if (createNewPackDialog.DataContext is CreateNewPackDialogViewModel dialogViewModel &&
             createNewPackDialog.ShowDialog() == true)
            {

                var newPack = new QuestionPack(viewModel.PackName, viewModel.Difficulty);
                var newPackViewModel = new QuestionPackViewModel(newPack);
                QuestionPacks.Add(newPackViewModel);
                ActivePack = newPackViewModel;
                await SaveCurrentPacks();
                SelectPack(newPackViewModel);
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
                            _localDataService.RemoveQuestionPack(QuestionPacks.Select(p => p.QuestionPack).ToList(), selectedPack.QuestionPack);
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
            return new QuestionPackViewModel(defaultPack);
        }
        private void PackOptionsDialog(object? obj)
        {
            PackOptionsDialog packOptionsDialog = new();

            var configurationViewModel = new ConfigurationViewModel(_mainWindowViewModel, this, _localDataService);
            packOptionsDialog.DataContext = configurationViewModel;

            packOptionsDialog.ShowDialog();
        }

        private async Task LoadQuestionPacks()
        {
            var dataService = new LocalDataService(this);

            var packs = await dataService.LoadQuestionPacks();

            foreach (var pack in packs)
            {
                var packViewModel = new QuestionPackViewModel(pack);
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
            var dataService = new LocalDataService(this);
            var existingPacks = await dataService.LoadQuestionPacks();

            var updatedPacks = QuestionPacks.Select(p => p.QuestionPack).ToList();
            foreach (var updatedPack in updatedPacks)
            {
                var existingPack = existingPacks.FirstOrDefault(p => p.Name == updatedPack.Name);
                if (existingPack != null)
                {
                    existingPacks.Remove(existingPack);
                }
                existingPacks.Add(updatedPack);
            }
            await dataService.SaveQuestionPacks(existingPacks);
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

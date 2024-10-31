using Labb3QuizApp.Command;
using Labb3QuizApp.Dialogs;
using Labb3QuizApp.Model;
using Labb3QuizApp.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Labb3QuizApp.ViewModel
{
    class MenuViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel? _mainWindowViewModel;
        private QuestionPackViewModel? _activePack;

        public DelegateCommand NavigateToQuiz { get; }
        public DelegateCommand NavigateToConfiguration { get; }
        public DelegateCommand OpenCreateNewPack { get; }
        public DelegateCommand OpenPackOptions { get; }
        public DelegateCommand SelectQuestionPack { get; }
        public ObservableCollection<QuestionPackViewModel> QuestionPacks { get; set; } = new ObservableCollection<QuestionPackViewModel>();

        public QuestionPackViewModel? ActivePack
        {
            get => _activePack;
            set
            {
                _activePack = value;
                RaisePropertyChanged(nameof(ActivePack));
            }
        }

        public MenuViewModel(MainWindowViewModel? mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
            OpenCreateNewPack = new DelegateCommand(CreateNewPackDialog);
            OpenPackOptions = new DelegateCommand(PackOptionsDialog);
            SelectQuestionPack = new DelegateCommand(SelectPack);
            NavigateToConfiguration = new DelegateCommand(obj =>
            {
                _mainWindowViewModel?.ShowConfigurationView.Execute(obj);
            });
            NavigateToQuiz = new DelegateCommand(obj =>
            {
                _mainWindowViewModel?.ShowPlayerView.Execute(obj);
            });
            LoadQuestionPacks();
        }

        private void CreateNewPackDialog(object? obj)
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
                SaveCurrentPacks();
            }
        }
        private void SelectPack(object? obj)
        {
            Debug.WriteLine($"Received object: {obj?.GetType()}");
            if (obj is QuestionPackViewModel selectedPack)
            {
                Debug.WriteLine($"Selected Pack Name: {selectedPack.Name}");
                ActivePack = selectedPack;
                _mainWindowViewModel.ActivePack = selectedPack;
            }
            else
            {
                Debug.WriteLine("No valid pack selected.");
            }
        }
        private void PackOptionsDialog(object? obj)
        {
            PackOptionsDialog packOptionsDialog = new();

            packOptionsDialog.ShowDialog();
        }
        private void LoadQuestionPacks()
        {
            var dataService = new LocalDataService();
            var packs = dataService.LoadQuestionPacks();
            foreach (var pack in packs)
            {
                QuestionPacks.Add(new QuestionPackViewModel(pack));
            }
        }
        public void SaveCurrentPacks()
        {
            var dataService = new LocalDataService();
            var existingPacks = dataService.LoadQuestionPacks();

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

            dataService.SaveQuestionPacks(existingPacks);
        }
    }
}

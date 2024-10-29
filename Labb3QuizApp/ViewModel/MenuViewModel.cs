using Labb3QuizApp.Command;
using Labb3QuizApp.Dialogs;
using Labb3QuizApp.Model;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Labb3QuizApp.ViewModel
{
    class MenuViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel? _mainWindowViewModel;
        private QuestionPackViewModel? _activePack;


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
        }

        private void CreateNewPackDialog(object? obj)
        {
            CreateNewPackDialog createNewPackDialog = new();
            var viewModel = new CreateNewPackDialogViewModel();

            if (createNewPackDialog.DataContext is CreateNewPackDialogViewModel dialogViewModel &&
             createNewPackDialog.ShowDialog() == true)
            {

                var newPack = new QuestionPack(viewModel.PackName, viewModel.Difficulty);
                var newPackViewModel = new QuestionPackViewModel(newPack);
                QuestionPacks.Add(newPackViewModel);
                ActivePack = newPackViewModel;
            }
        }
        private void SelectPack(object? obj)
        {
            if (obj is QuestionPackViewModel selectedPack)
            {
                Debug.WriteLine($"Selected Pack Name: {selectedPack.Name}");
                _mainWindowViewModel.ActivePack = selectedPack;
            }
        }
        private void PackOptionsDialog(object? obj)
        {
            PackOptionsDialog packOptionsDialog = new();

            packOptionsDialog.ShowDialog();
        }
    }

}

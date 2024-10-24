using Labb3QuizApp.Command;
using Labb3QuizApp.Dialogs;

namespace Labb3QuizApp.ViewModel
{
    class MenuViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel? mainWindowViewModel;

        public DelegateCommand OpenCreateNewPack { get; }

        public MenuViewModel(MainWindowViewModel? mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel;
            OpenCreateNewPack = new DelegateCommand(CreateNewPackHandler);

        }
        private void CreateNewPackHandler(object? obj)
        {
            CreateNewPackDialog createNewPackDialog = new();

            createNewPackDialog.ShowDialog();
        }
    }
}

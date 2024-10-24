using Labb3QuizApp.Command;
using Labb3QuizApp.Dialogs;

namespace Labb3QuizApp.ViewModel
{
    class ConfigurationViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel? mainWindowViewModel;
        public DelegateCommand OpenPackOptions { get; }
        private string ActiveQuestion { get; }

        public QuestionPackViewModel? ActivePack { get => mainWindowViewModel?.ActivePack; }
        public ConfigurationViewModel(MainWindowViewModel? mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel;
            OpenPackOptions = new DelegateCommand(PackOptionsHandler);
        }

        private void PackOptionsHandler(object? obj)
        {
            PackOptionsDialog packOptionsDialog = new();

            packOptionsDialog.ShowDialog();
        }

    }
}

namespace Labb3QuizApp.ViewModel
{
    class MenuViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel? mainWindowViewModel;

        public MenuViewModel(MainWindowViewModel? mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel;
        }
    }
}

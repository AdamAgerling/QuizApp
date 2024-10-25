using Labb3QuizApp.Command;
using Labb3QuizApp.Model;

namespace Labb3QuizApp.ViewModel
{
    class ConfigurationViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel? mainWindowViewModel;

        public MenuViewModel MenuViewModel { get; }
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

        public QuestionPackViewModel? ActivePack { get => mainWindowViewModel?.ActivePack; }

        public ConfigurationViewModel(MainWindowViewModel? mainWindowViewModel, MenuViewModel? menuViewModel)
        {
            MenuViewModel = menuViewModel;
            this.mainWindowViewModel = mainWindowViewModel;
            AddQuestion = new DelegateCommand(AddQuestionHandler);
            RemoveQuestion = new DelegateCommand(RemoveQuestionHandler, CanRemoveQuestion);
        }
        private void RemoveQuestionHandler(object? obj)
        {
            if (SelectedQuestion != null)
            {
                mainWindowViewModel.ActivePack.Questions.Remove(SelectedQuestion);
                SelectedQuestion = null;

            }
        }
        private bool CanRemoveQuestion(object? obj) => SelectedQuestion != null;

        private void AddQuestionHandler(object? obj)
        {
            var newQuestion = new Question("New Question", "", "", "", "");
            mainWindowViewModel?.ActivePack?.Questions.Add(newQuestion);
            SelectedQuestion = newQuestion;
        }

    }
}

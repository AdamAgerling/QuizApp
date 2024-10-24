using Labb3QuizApp.Command;
using System.Windows.Threading;

namespace Labb3QuizApp.ViewModel
{
    class PlayerViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel? mainWindowViewModel;
        public DelegateCommand UpdateButtonCommand { get; }

        private DispatcherTimer _timer;

        //private string _testData;

        //public string TestData
        //{
        //    get => _testData;
        //    private set
        //    {
        //        _testData = value;
        //        RaisePropertyChanged();
        //    }
        //}

        public PlayerViewModel(MainWindowViewModel? mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel;

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += QuestionTimer;
            _timer.Start();

            //UpdateButtonCommand = new DelegateCommand(UpdateButton, CanUpdateButton);

        }

        private void QuestionTimer(object? sender, EventArgs e)
        {
            //_timer += 
        }

        //private bool CanUpdateButton(object? arg)
        //{
        //    throw new NotImplementedException();
        //}

        //private void UpdateButton(object obj)
        //{
        //    //TestData += "x";
        //}


    }
}

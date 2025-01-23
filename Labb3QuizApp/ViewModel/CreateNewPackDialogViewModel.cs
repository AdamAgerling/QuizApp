using Labb3QuizApp.Model;
using System.Collections.ObjectModel;

namespace Labb3QuizApp.ViewModel
{
    public class CreateNewPackDialogViewModel : ViewModelBase
    {
        private string _packName;
        private Difficulty _difficulty;
        private int _timeLimitInSeconds;
        public ObservableCollection<Category> Categories { get; set; }
        public string SelectedCategoryId { get; set; }
        public CategoryViewModel CategoryViewModel { get; }

        public QuestionPackViewModel ActivePack { get; set; }

        //public Category SelectedCategory
        //{
        //    get => _selectedCategory;
        //    set
        //    {
        //        if (_selectedCategory != value)
        //        {
        //            _selectedCategory = value;
        //            RaisePropertyChanged(nameof(SelectedCategory));

        //            if (value != null && ActivePack != null)
        //            {
        //                ActivePack.SelectedCategory = value;
        //            }
        //        }
        //    }
        //}

        public string PackName
        {
            get => _packName;
            set
            {
                if (_packName != value)
                {
                    _packName = value;
                    RaisePropertyChanged(nameof(PackName));
                }
            }
        }

        public Difficulty Difficulty
        {
            get => _difficulty;
            set
            {
                if (_difficulty != value)
                {
                    _difficulty = value;
                    RaisePropertyChanged(nameof(Difficulty));
                }
            }
        }


        public int TimeLimitInSeconds
        {
            get => _timeLimitInSeconds;
            set
            {
                if (_timeLimitInSeconds != value)
                {
                    _timeLimitInSeconds = value;
                    RaisePropertyChanged(nameof(TimeLimitInSeconds));
                }
            }
        }
        public ObservableCollection<Difficulty> SelectDifficulty { get; }

        public CreateNewPackDialogViewModel(CategoryViewModel categoryViewModel)
        {

            PackName = "New Pack";
            SelectDifficulty = new ObservableCollection<Difficulty>(Enum.GetValues(typeof(Difficulty)).Cast<Difficulty>());
            TimeLimitInSeconds = 30;
            CategoryViewModel = categoryViewModel ?? throw new ArgumentNullException(nameof(categoryViewModel));
            Categories = categoryViewModel.Categories;
            SelectedCategoryId = Categories.FirstOrDefault()?.Id.ToString(); // Ändra till Name imorgon och städa upp kod imorgon.

            if (CategoryViewModel.Categories.Any())
            {
                CategoryViewModel.SelectedCategory = CategoryViewModel.Categories.First();

            }
        }

    }
}


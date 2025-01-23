using Labb3QuizApp.Model;
using MongoDB.Bson;
using System.Collections.ObjectModel;

namespace Labb3QuizApp.ViewModel
{
    public class QuestionPackViewModel : ViewModelBase
    {
        private readonly QuestionPack _model;
        public QuestionPack QuestionPack => _model;
        private Category? _selectedCategory;
        private string? _selectedCategoryId;
        public ObservableCollection<Question> Questions { get; set; } = new ObservableCollection<Question>();

        public string Name
        {
            get => _model.Name;
            set
            {
                _model.Name = value;
                RaisePropertyChanged(nameof(Name));
            }
        }

        public string? SelectedCategoryId
        {
            get => _selectedCategoryId;
            set
            {
                if (_selectedCategoryId != value)
                {
                    _selectedCategoryId = value;
                    RaisePropertyChanged(nameof(SelectedCategoryId));
                    UpdateSelectedCategory();
                }
            }
        }

        public Difficulty Difficulty
        {
            get => _model.Difficulty;
            set
            {
                _model.Difficulty = value;
                RaisePropertyChanged(nameof(Difficulty));
            }
        }

        public Category? SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (_selectedCategory != value)
                {
                    _selectedCategory = value;
                    SelectedCategoryId = value?.Id.ToString();
                    RaisePropertyChanged(nameof(SelectedCategory));
                }
            }
        }

        public int TimeLimitInSeconds
        {
            get => _model.TimeLimitInSeconds;
            set
            {
                _model.TimeLimitInSeconds = value;
                RaisePropertyChanged(nameof(TimeLimitInSeconds));
            }
        }

        private void UpdateSelectedCategory()
        {
            if (ObjectId.TryParse(SelectedCategoryId, out var objectId))
            {
                SelectedCategory = _categoryList.FirstOrDefault(c => c.Id == objectId);
            }
            else
            {
                SelectedCategory = null;
            }
        }

        private readonly IEnumerable<Category> _categoryList;

        public QuestionPackViewModel(QuestionPack model, IEnumerable<Category> categories)
        {
            _model = model;
            Questions = new ObservableCollection<Question>(model.Questions);
            _categoryList = categories;
            SelectedCategoryId = model.SelectedCategoryId;
            UpdateSelectedCategory();
        }
        public override string ToString()
        {
            return $"{Name} ({SelectedCategory?.Name ?? "No Category"}) ({Difficulty})";
        }
    }
}

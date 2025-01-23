using Labb3QuizApp.Command;
using Labb3QuizApp.Model;
using Labb3QuizApp.Services;
using Labb3QuizApp.ViewModel;
using System.Collections.ObjectModel;
using System.Diagnostics;

public class CategoryViewModel : ViewModelBase
{
    private readonly MongoDataService _mongoDataService;
    private ObservableCollection<Category> _categories;
    private readonly QuestionPackViewModel _activePack;
    public QuestionPackViewModel ActivePack
    {
        get => _activePack;
    }
    public ObservableCollection<Category> Categories { get; set; }

    private Category? _selectedCategory;
    public Category? SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            _selectedCategory = value;
            RaisePropertyChanged(nameof(SelectedCategory));
        }
    }

    public DelegateCommand AddCategoryCommand { get; }
    public DelegateCommand UpdateCategoryCommand { get; }
    public DelegateCommand RemoveCategoryCommand { get; }

    public CategoryViewModel(MongoDataService mongoDataService)
    {
        _mongoDataService = mongoDataService ?? throw new ArgumentNullException(nameof(mongoDataService));

        Categories = new ObservableCollection<Category>();
        LoadCategories(mongoDataService);

        AddCategoryCommand = new DelegateCommand(param => AddCategory((Category)param));
        UpdateCategoryCommand = new DelegateCommand(param => UpdateCategory((Category)param));
        RemoveCategoryCommand = new DelegateCommand(param => RemoveCategory((string)param));
    }

    public async void LoadCategories(MongoDataService mongoDataService)
    {
        var categories = await _mongoDataService.GetCategories();
        Categories.Clear();

        foreach (var category in categories)
        {
            Categories.Add(category);
        }
        Debug.WriteLine($"Categories count: {Categories.Count}");
        foreach (var category in Categories)
        {
            Debug.WriteLine($"Category: {category.Name}");
        }
    }

    public async void AddCategory(Category newCategory)
    {
        await _mongoDataService.AddCategory(newCategory);
        Categories.Add(newCategory);
    }

    public async void UpdateCategory(Category updatedCategory)
    {
        if (updatedCategory != null && !string.IsNullOrEmpty(updatedCategory.Name))
        {
            try
            {
                await _mongoDataService.UpdateCategory(updatedCategory);
                var existingCategory = Categories.FirstOrDefault(c => c.Id == updatedCategory.Id);
                if (existingCategory != null)
                {
                    existingCategory.Name = updatedCategory.Name;
                    RaisePropertyChanged(nameof(Categories));
                }
                Debug.WriteLine($"Category updated: {updatedCategory.Name}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating category: {ex.Message}");
            }
        }
        else
        {
            Debug.WriteLine("Invalid category or category name is empty. Cannot update.");
        }
    }

    public async void RemoveCategory(string categoryId)
    {
        if (!string.IsNullOrEmpty(categoryId))
        {
            try
            {
                await _mongoDataService.RemoveCategory(categoryId);
                var categoryToRemove = Categories.FirstOrDefault(c => c.Id.ToString() == categoryId);
                if (categoryToRemove != null)
                {
                    Categories.Remove(categoryToRemove);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error removing category: {ex.Message}");
            }
        }
        else
        {
            Debug.WriteLine("Category ID is null or empty. Cannot remove category.");
        }
    }
}

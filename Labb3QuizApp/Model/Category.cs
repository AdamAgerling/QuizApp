using Labb3QuizApp.ViewModel;
using MongoDB.Bson;

namespace Labb3QuizApp.Model
{
    public class Category : ViewModelBase
    {
        private string _name;

        public ObjectId Id { get; set; }
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    RaisePropertyChanged(nameof(Name));
                }
            }
        }

        public Category()
        {

        }

        public Category(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}


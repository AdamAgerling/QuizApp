using Labb3QuizApp.ViewModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Labb3QuizApp.Model
{
    public enum Difficulty { Easy, Medium, Hard };
    public class QuestionPack : ViewModelBase
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public Difficulty Difficulty { get; set; }
        public int TimeLimitInSeconds { get; set; }
        public string? SelectedCategoryId { get; set; }
        public List<Question> Questions { get; set; }

        public QuestionPack(string name, Difficulty difficulty = Difficulty.Easy, int timeLimitInSeconds = 30)
        {
            Name = name;
            Difficulty = difficulty;
            TimeLimitInSeconds = timeLimitInSeconds;
            Questions = new List<Question>();
        }
    }
}
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Labb3QuizApp.Model
{
    enum Difficulty { Easy, Medium, Hard };
    enum Category { History, Science, Geography, Math };

    class QuestionPack
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public Difficulty Difficulty { get; set; }
        public int TimeLimitInSeconds { get; set; }
        public Category Category { get; set; }
        public List<Question> Questions { get; set; }

        public QuestionPack(string name, Difficulty difficulty = Difficulty.Easy, int timeLimitInSeconds = 30, Category category = Category.History)
        {
            Name = name;
            Difficulty = difficulty;
            TimeLimitInSeconds = timeLimitInSeconds;
            Category = category;
            Questions = new List<Question>();
        }
    }
}

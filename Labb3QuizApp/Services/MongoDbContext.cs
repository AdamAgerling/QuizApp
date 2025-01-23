using Labb3QuizApp.Model;
using MongoDB.Driver;

namespace Labb3QuizApp.Services
{
    public class MongoDbContext
    {
        public readonly IMongoDatabase _database;

        public MongoDbContext(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<QuestionPack> QuestionPacks => _database.GetCollection<QuestionPack>("QuestionPacks");
        public IMongoCollection<Category> Categories => _database.GetCollection<Category>("Categories");

        public async Task AddQuestionPackAsync(QuestionPack questionPack)
        {
            var collection = _database.GetCollection<QuestionPack>("QuestionPacks");
            await collection.InsertOneAsync(questionPack);
        }
    }
}

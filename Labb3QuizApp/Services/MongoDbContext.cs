using Labb3QuizApp.Model;
using MongoDB.Driver;

namespace Labb3QuizApp.Services
{
    class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);

            _database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<QuestionPack> QuestionPacks => _database.GetCollection<QuestionPack>("QuestionPacks");
        public IMongoCollection<string> Categories => _database.GetCollection<String>("Categories");
    }
}

using Labb3QuizApp.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Diagnostics;

namespace Labb3QuizApp.Services
{
    public class MongoDataService
    {
        public readonly MongoDbContext _dbContext;
        private readonly IMongoCollection<QuestionPack> _collection;

        public MongoDataService(string connectionString, string databaseName, string collectionName)
        {
            _dbContext = new MongoDbContext(connectionString, databaseName);
            _collection = _dbContext._database.GetCollection<QuestionPack>(collectionName);
        }

        public async Task SaveQuestions(List<Question> questions, string packName)
        {
            var collection = _dbContext.QuestionPacks;
            var filter = Builders<QuestionPack>.Filter.Eq(p => p.Name, packName);
            var update = Builders<QuestionPack>.Update.Set(p => p.Questions, questions);

            var result = await collection.UpdateOneAsync(filter, update);
            if (result.MatchedCount == 0)
            {
                var newPack = new QuestionPack(packName) { Questions = questions };
                await collection.InsertOneAsync(newPack);
            }
        }

        public async Task SaveMultipleQuestionPacks(List<QuestionPack> newPacks)
        {
            var collection = _dbContext.QuestionPacks;

            foreach (var newPack in newPacks)
            {
                var filter = Builders<QuestionPack>.Filter.Eq(p => p.Name, newPack.Name);
                var existingPack = await collection.Find(filter).FirstOrDefaultAsync();

                if (existingPack != null)
                {
                    await collection.ReplaceOneAsync(filter, newPack);
                }
                else
                {
                    await collection.InsertOneAsync(newPack);
                }
            }
        }

        public async Task<List<QuestionPack>> LoadQuestionPacks()
        {
            var collection = _dbContext.QuestionPacks;
            var questionPacks = await collection.Find(FilterDefinition<QuestionPack>.Empty).ToListAsync();

            foreach (var pack in questionPacks)
            {
                Console.WriteLine($"Pack Name: {pack.Name}, Question count: {pack.Questions.Count}");
            }
            return questionPacks;
        }

        public async Task RemoveQuestionPack(QuestionPack packToRemove)
        {
            if (packToRemove != null && packToRemove.Name != "Default Pack")
            {
                var collection = _dbContext.QuestionPacks;
                var filter = Builders<QuestionPack>.Filter.Eq(p => p.Name, packToRemove.Name);
                var result = await collection.DeleteOneAsync(filter);
            }
            else
            {
                Debug.WriteLine("Default Pack cannot be removed.");
            }
        }

        public async Task AddCategory(Category category)
        {
            var collection = _dbContext._database.GetCollection<Category>("Categories");
            await collection.InsertOneAsync(category);
        }

        public async Task UpdateCategory(Category category)
        {
            if (category.Id == ObjectId.Empty)
            {
                throw new ArgumentException("Category ID cannot be empty.");
            }

            var collection = _dbContext._database.GetCollection<Category>("Categories");
            var filter = Builders<Category>.Filter.Eq(c => c.Id, category.Id);

            await collection.ReplaceOneAsync(filter, category);
        }

        public async Task RemoveCategory(string categoryId)
        {
            if (!ObjectId.TryParse(categoryId, out ObjectId objectId))
            {
                throw new FormatException($"The category ID '{categoryId}' is not a valid ObjectId.");
            }

            var collection = _dbContext._database.GetCollection<Category>("Categories");
            var filter = Builders<Category>.Filter.Eq(c => c.Id, objectId);

            await collection.DeleteOneAsync(filter);
        }

        public async Task<List<Category>> GetCategories()
        {
            var collection = _dbContext._database.GetCollection<Category>("Categories");
            return await collection.Find(_ => true).ToListAsync();
        }

        public async Task SetupDefaultCategories()
        {
            var collection = _dbContext._database.GetCollection<Category>("Categories");

            var existingCategories = await collection.Find(_ => true).ToListAsync();
            if (!existingCategories.Any())
            {
                var defaultCategories = new List<Category>
        {
            new Category { Name = "History" },
            new Category { Name = "Science" },
            new Category { Name = "Geography" },
            new Category { Name = "Sports" }
        };
                await collection.InsertManyAsync(defaultCategories);
            }
        }
    }
}


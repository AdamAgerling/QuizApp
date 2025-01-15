using Labb3QuizApp.Model;
using MongoDB.Driver;

namespace Labb3QuizApp.Services
{
    internal class MongoDataService
    {
        private readonly MongoDbContext _dbContext;


        public MongoDataService(string connectionString, string databaseName)
        {
            _dbContext = new MongoDbContext(connectionString, databaseName);
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

        public async Task SaveQuestionPacks(List<QuestionPack> newPacks)
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
                Console.WriteLine("Default Pack cannot be removed.");
            }
        }

        public async Task AddCategory(string category)
        {
            if (!_dbContext.Categories.AsQueryable().Contains(category))
            {
                await _dbContext.Categories.InsertOneAsync(category);
            }
        }

        public async Task<List<string>> LoadCategories()
        {
            return await _dbContext.Categories.Find(_ => true).ToListAsync();
        }

        public async Task RemoveCategory(string category)
        {
            var filter = Builders<string>.Filter.Eq(c => c, category);
            await _dbContext.Categories.DeleteOneAsync(filter);
        }

    }
}

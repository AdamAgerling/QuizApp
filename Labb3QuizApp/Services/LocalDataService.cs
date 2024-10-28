using Labb3QuizApp.Model;
using System.IO;
using System.Text.Json;

namespace Labb3QuizApp.Services
{

    internal class LocalDataService
    {
        private readonly string filePath = "questions.json";

        private JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            IncludeFields = true
        };

        public void SaveQuestions(IEnumerable<Question> questions)
        {
            var json = JsonSerializer.Serialize(questions, options);
            File.WriteAllText(filePath, json);
        }

        public List<Question> LoadQuestions()
        {
            if (!File.Exists(filePath))
            {
                return new List<Question>();
            }

            var json = File.ReadAllText(filePath);
            return string.IsNullOrWhiteSpace(json) ? new List<Question>() :
                JsonSerializer.Deserialize<List<Question>>(json, options) ?? new List<Question>();
        }
    }
}

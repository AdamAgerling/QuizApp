using Labb3QuizApp.Model;
using System.IO;
using System.Text.Json;

namespace Labb3QuizApp.Services
{

    internal class LocalDataService
    {
        private readonly string filePath = "Questionpacks.json";

        private JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            IncludeFields = true
        };

        public void SaveQuestions(List<Question> questions, string packName)
        {
            var existingPacks = LoadQuestionPacks();

            var packToUpdate = existingPacks.FirstOrDefault(p => p.Name == packName);
            if (packToUpdate != null)
            {
                packToUpdate.Questions = questions;
            }
            else
            {
                var newPack = new QuestionPack(packName) { Questions = questions };
                existingPacks.Add(newPack);
            }

            var json = JsonSerializer.Serialize(new { QuestionPacks = existingPacks }, options);
            File.WriteAllText(filePath, json);
        }

        public void SaveQuestionPacks(List<QuestionPack> newPacks)
        {
            var existingPacks = LoadQuestionPacks();

            foreach (var newPack in newPacks)
            {
                var existingPack = existingPacks.FirstOrDefault(p => p.Name == newPack.Name);
                if (existingPack != null)
                {
                    existingPacks.Remove(existingPack);
                }
                existingPacks.Add(newPack);
            }
            var json = JsonSerializer.Serialize(new { QuestionPacks = existingPacks }, options);
            File.WriteAllText(filePath, json);
        }

        public List<QuestionPack> LoadQuestionPacks()
        {
            try
            {
                var json = File.ReadAllText(filePath);
                var result = JsonSerializer.Deserialize<RootObject>(json, options);
                return result?.QuestionPacks ?? new List<QuestionPack>();
            }
            catch (FileNotFoundException)
            {
                return new List<QuestionPack>();
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON Error: {ex.Message}");
                return new List<QuestionPack>();
            }
        }
        public List<Question> LoadQuestionsInPack(string packName)
        {
            var json = File.ReadAllText($"{packName}_questions.json");
            return JsonSerializer.Deserialize<List<Question>>(json, options) ?? new List<Question>();
        }

        public class RootObject
        {
            public List<QuestionPack> QuestionPacks { get; set; } = new List<QuestionPack>();
        }
    }
}

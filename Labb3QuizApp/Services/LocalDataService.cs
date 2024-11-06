using Labb3QuizApp.Model;
using Labb3QuizApp.ViewModel;
using System.IO;
using System.Text.Json;

namespace Labb3QuizApp.Services
{

    internal class LocalDataService
    {
        private readonly string filePath = "Questionpacks.json";
        private MenuViewModel? _menuViewModel;
        private JsonSerializerOptions options = new JsonSerializerOptions
        {
            WriteIndented = true,
            IncludeFields = true
        };

        public LocalDataService(MenuViewModel menuViewModel)
        {
            _menuViewModel = menuViewModel;
        }

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
            if (existingPacks.Count == 0)
            {
                var defaultPack = new QuestionPack("Default Pack", Difficulty.Easy, 30);
                existingPacks.Add(defaultPack);
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
                if (result == null || result.QuestionPacks == null)
                {
                    return new List<QuestionPack>();
                }
                return result.QuestionPacks;
            }
            catch (JsonException ex)
            {

                return new List<QuestionPack> { new QuestionPack("Default Pack", Difficulty.Easy, 30) };
            }
            catch (FileNotFoundException)
            {
                return new List<QuestionPack> { new QuestionPack("Default Pack", Difficulty.Easy, 30) };
            }
        }
        public void RemoveQuestionPack(List<QuestionPack> packs, QuestionPack packToRemove)
        {

            if (packToRemove != null && packToRemove.Name != "Default Pack")
            {
                packs.Remove(packToRemove);
                var json = JsonSerializer.Serialize(new { QuestionPacks = packs }, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);
            }
            else
            {
                Console.WriteLine("Default Pack cannot be removed.");
            }
        }
        public class RootObject
        {
            public List<QuestionPack> QuestionPacks { get; set; } = new List<QuestionPack>();
        }
    }
}

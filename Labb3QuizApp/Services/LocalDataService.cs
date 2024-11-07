using Labb3QuizApp.Model;
using Labb3QuizApp.ViewModel;
using System.IO;
using System.Text.Json;

namespace Labb3QuizApp.Services
{

    internal class LocalDataService
    {
        private readonly string filePath;
        private MenuViewModel? _menuViewModel;
        private JsonSerializerOptions options = new JsonSerializerOptions
        {
            WriteIndented = true,
            IncludeFields = true
        };

        public LocalDataService(MenuViewModel menuViewModel)
        {
            _menuViewModel = menuViewModel;
            var appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "QuizApp");
            Directory.CreateDirectory(appDataFolder);
            filePath = Path.Combine(appDataFolder, "Questionpacks.json");
        }

        public async Task SaveQuestions(List<Question> questions, string packName)
        {
            var existingPacks = await LoadQuestionPacks();

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

        public async Task SaveQuestionPacks(List<QuestionPack> newPacks)
        {
            var existingPacks = await LoadQuestionPacks();

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
            await File.WriteAllTextAsync(filePath, json);
        }

        public async Task<List<QuestionPack>> LoadQuestionPacks()
        {
            try
            {
                var json = await File.ReadAllTextAsync(filePath);
                var result = JsonSerializer.Deserialize<RootObject>(json, options);
                if (result == null || result.QuestionPacks == null)
                {
                    return new List<QuestionPack>();
                }
                return result.QuestionPacks;
            }

            catch (FileNotFoundException)
            {
                return new List<QuestionPack> { new QuestionPack("Default Pack", Difficulty.Easy, 30) };
            }
        }
        public async Task RemoveQuestionPack(List<QuestionPack> packs, QuestionPack packToRemove)
        {

            if (packToRemove != null && packToRemove.Name != "Default Pack")
            {
                packs.Remove(packToRemove);
                var json = JsonSerializer.Serialize(new { QuestionPacks = packs }, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(filePath, json);
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

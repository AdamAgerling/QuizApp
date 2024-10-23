namespace Labb3QuizApp.Model
{
    enum Difficulty { Easy, Medium, Hard };
    class QuestionPack
    {
        public QuestionPack(string name, Difficulty difficulty = Difficulty.Easy, int timeLimitInSeconds = 30)
        {
            Name = name;
            Difficulty = difficulty;
            TimeLimitInSeconds = timeLimitInSeconds;
            Questions = new List<Question>();
        }
        public string Name { get; set; }
        public Difficulty Difficulty { get; set; }
        public int TimeLimitInSeconds { get; set; }
        public List<Question> Questions { get; set; }

    }
}

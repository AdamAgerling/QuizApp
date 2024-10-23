namespace Labb3QuizApp.Model
{
    class Question
    {
        public Question(string query, string correctAnswer, string incorrectAnswer1,
            string incorrectAnswer2, string incorrectAnswer3)
        {
            Query = query;
            CorrectAnswer = correctAnswer;
            InCorrectAnswers = new string[3] { incorrectAnswer1, incorrectAnswer2, incorrectAnswer3 };
        }
        public string Query { get; set; }
        public string CorrectAnswer { get; set; }
        public string[] InCorrectAnswers { get; set; }
    }
}

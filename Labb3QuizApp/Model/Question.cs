namespace Labb3QuizApp.Model
{
    class Question
    {
        public string QuizQuestion { get; set; }
        public string CorrectAnswer { get; set; }
        public string[] InCorrectAnswers { get; set; }
        public string[] ShuffledAnswers { get; private set; }

        public Question(string quizQuestion, string correctAnswer, string incorrectAnswer1,
            string incorrectAnswer2, string incorrectAnswer3)
        {
            QuizQuestion = quizQuestion;
            CorrectAnswer = correctAnswer;
            InCorrectAnswers = new string[3] { incorrectAnswer1, incorrectAnswer2, incorrectAnswer3 };
            ShuffleAnswers();
        }
        public Question()
        {
            InCorrectAnswers = new string[3];
            ShuffledAnswers = Array.Empty<string>();
        }

        private void ShuffleAnswers()
        {
            var answers = InCorrectAnswers.Append(CorrectAnswer).ToList();
            ShuffledAnswers = answers.OrderBy(_ => Guid.NewGuid()).ToArray();
        }


    }
}
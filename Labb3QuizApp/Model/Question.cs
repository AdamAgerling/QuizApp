﻿namespace Labb3QuizApp.Model
{
    class Question
    {
        public string QuizQuestion { get; set; }
        public string CorrectAnswer { get; set; }
        public string[] InCorrectAnswers { get; set; }

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
        }

        private List<String> ShuffleAnswers()
        {
            var answers = InCorrectAnswers.Append(CorrectAnswer).ToList();
            return answers.OrderBy(_ => Guid.NewGuid()).ToList();
        }
    }
}

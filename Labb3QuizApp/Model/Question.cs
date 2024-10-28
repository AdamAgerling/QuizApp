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
        }
        public Question()
        {
            InCorrectAnswers = new string[3];
        }
    }
}
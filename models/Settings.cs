using System;

namespace PookieBears.Dnn.Dnn_PookieBears_HelloWorld.Models
{
    public class Settings
    {
        public bool Setting1 { get; set; }
        public DateTime Setting2 { get; set; }
        /// <summary>
        /// A quiz kérdések számát határozza meg
        /// </summary>
        public int NumberOfQuestions { get; set; } = 10;

        /// <summary>
        /// A quiz modul címe
        /// </summary>
        public string QuizTitle { get; set; } = "Makett Kiválasztó";
    }
}

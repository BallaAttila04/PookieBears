using System;

namespace PookieBears.Dnn.Dnn_PookieBears_HelloWorld.Models
{
    public class Settings
    {
        public bool Setting1 { get; set; }
        public DateTime Setting2 { get; set; }
        /// <summary>
        /// A quiz k�rd�sek sz�m�t hat�rozza meg
        /// </summary>
        public int NumberOfQuestions { get; set; } = 10;

        /// <summary>
        /// A quiz modul c�me
        /// </summary>
        public string QuizTitle { get; set; } = "Makett Kiv�laszt�";
    }
}

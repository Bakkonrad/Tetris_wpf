using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris_wpf
{
    public class HighestScore
    {
        //public int savedScore;
        private string highestScoreFilePath = @"C:\Users\konra\Documents\pliki\programowanie\C# .NET\Tetris_wpf\Tetris_wpf\highestScore.txt"; // ścieżka do pliku z wynikami

        public void SaveScore(int score)
        {
            using (StreamWriter writer = new StreamWriter(highestScoreFilePath, false))
            {
                //czyszczenie zawartości pliku tekstowego
                writer.WriteLine(score.ToString());
            }
        }

        public int LoadScore()
        {
            if (!File.Exists(highestScoreFilePath))
            {
                return 0; // zwracamy domyślną wartość (np. 0) jeśli plik z wynikiem nie istnieje
            }

            string scoreString = File.ReadAllText(highestScoreFilePath);
            int score;

            if (int.TryParse(scoreString, out score))
            {
                return score;
            }
            else
            {
                return 0; // zwracamy domyślną wartość jeśli nie udało się wczytać poprawnego wyniku
            }


        }
    }
}

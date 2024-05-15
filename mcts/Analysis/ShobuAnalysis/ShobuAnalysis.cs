using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using mcts.Games.Interfaces;
using mcts.Games.Shobu;

namespace mcts.Analysis.ShobuAnalysis
{
    public static class ShobuAnalysis
    {
        public static List<int> GameLength(int numOfGames)
        {
            Random random = new Random();
            int toPlay = numOfGames;
            List<int> gameLengths = new List<int>();
            while (toPlay-- > 0)
            {
                Shobu shobu = new Shobu();
                int length = 0;
                GameResult result = shobu.Result(shobu.PlayerToGo());
                while (!result.IsOver)
                {
                    if (length >= 1000) break;
                    List<IMove> moves = shobu.GetLegalMoves();
                    if (moves.Count > 0) shobu.MakeMove(moves[random.Next(moves.Count)]);
                    result = shobu.Result(shobu.PlayerToGo());
                    length++;
                }
                Console.WriteLine($"{numOfGames - toPlay} : {length}");
                gameLengths.Add(length);
            }
            return gameLengths;
        }

        public static List<int> WinRate(int numOfGames)
        {
            Random random = new Random();
            int toPlay = numOfGames;

            int whiteWin = 0;
            int blackWin = 0;
            int timeout = 0;

            while (toPlay-- > 0)
            {
                Shobu shobu = new Shobu();
                int length = 0;
                GameResult result = shobu.Result(shobu.PlayerToGo());
                while (!result.IsOver)
                {
                    if (length >= 1000)
                    {
                        timeout++;
                        break;
                    }
                    List<IMove> moves = shobu.GetLegalMoves();
                    if (moves.Count > 0) shobu.MakeMove(moves[random.Next(moves.Count)]);
                    result = shobu.Result(shobu.PlayerToGo());
                    length++;
                }
                result = shobu.Result(1);
                if (result.Score > 0) 
                {
                    whiteWin++;
                } 
                else if (result.Score < 0)
                {
                    blackWin++;
                }
                Console.WriteLine($"{numOfGames - toPlay} : {whiteWin}");
            }
            return new List<int>() { blackWin, whiteWin, timeout };
        }

        public static void SaveToCsv(List<int> list, string fileName)
        {
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                writer.WriteLine("Black wins");
                writer.WriteLine("White wins");
                writer.WriteLine("Stalemates");
                foreach (var length in list)
                {
                    writer.WriteLine(length);
                }
            }
            Console.WriteLine($"Data saved to {fileName}");
        }
    }
}

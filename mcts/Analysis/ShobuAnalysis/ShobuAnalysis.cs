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
        public static void VersionConsistenceTest()
        {
            Random random = new Random(2137);
            Shobu shobu = new Shobu();
            int length = 0;
            GameResult result = shobu.Result(shobu.PlayerToGo());
            while (!result.IsOver)
            {
                if (length >= 1000) break;
                List<IMove> moves = shobu.GetLegalMoves();
                moves = moves.OrderBy(x => x.GetHashCode()).ToList();
                Console.WriteLine(moves.Count);
                if (moves.Count > 0) shobu.MakeMove(moves[random.Next(moves.Count)]);
                result = shobu.Result(shobu.PlayerToGo());
                length++;
            }
        }

        public static List<int> PseudoRandomGame(int numOfGames)
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
                    shobu.MakeMove(shobu.PseudoRandomMove(random));
                    result = shobu.Result(shobu.PlayerToGo());
                    length++;
                }
                Console.WriteLine($"{numOfGames - toPlay} : {length}");
                gameLengths.Add(length);
            }
            return gameLengths;
        }

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

        public static List<double> BranchingFactor(int numOfGames)
        {
            Random random = new Random();
            int toPlay = numOfGames;
            List<List<int>> possibleMoves = new List<List<int>>();

            while (toPlay-- > 0)
            {
                possibleMoves.Add(new List<int>());
                Shobu shobu = new Shobu();
                int length = 0;
                GameResult result = shobu.Result(shobu.PlayerToGo());

                while (!result.IsOver)
                {
                    if (length >= 1000) break;
                    List<IMove> moves = shobu.GetLegalMoves();
                    possibleMoves[possibleMoves.Count - 1].Add(moves.Count);
                    if (moves.Count > 0) shobu.MakeMove(moves[random.Next(moves.Count)]);
                    result = shobu.Result(shobu.PlayerToGo());
                    length++;
                }

                Console.WriteLine($"{numOfGames - toPlay} : {length}");
            }
            List<double> res = new List<double>();
            int maxLength = possibleMoves.Max(movesList => movesList.Count);
            for (int i = 0; i < maxLength; i++)
            {
                double sum = 0;
                int count = 0;
                foreach (var movesList in possibleMoves)
                {
                    if (i < movesList.Count)
                    {
                        sum += movesList[i];
                        count++;
                    }
                }
                res.Add(count > 0 ? sum / count : 0);
            }
            return res;
        }

        public static void SaveWinrates(List<int> list, string fileName)
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

        public static void SaveLengths(List<int> list, string fileName)
        {
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                writer.WriteLine("Data");
                foreach (var length in list)
                {
                    writer.WriteLine(length);
                }
            }
            Console.WriteLine($"Data saved to {fileName}");
        }

        public static void SaveBranching(List<double> list, string fileName)
        {
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                writer.WriteLine("MoveIndex,AveragePossibleMoves");
                for (int i = 0; i < list.Count; i++)
                {
                    writer.WriteLine($"{i},{list[i]}");
                }
            }
            Console.WriteLine($"Data saved to {fileName}");
        }
    }
}

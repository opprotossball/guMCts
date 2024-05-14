using System;
using System.Collections.Generic;
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
        public static void GameLength(int numOfGames)
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
                    List<IMove> moves = shobu.GetLegalMoves();
                    if (moves.Count > 0) shobu.MakeMove(moves[random.Next(moves.Count)]); length++;
                    result = shobu.Result(shobu.PlayerToGo());
                    length++;
                }
                gameLengths.Add(length);
            }
            foreach (int l in gameLengths)
            {
                Console.WriteLine(l);
            }
        }
    }
}

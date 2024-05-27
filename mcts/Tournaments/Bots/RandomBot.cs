using mcts.Bot;
using mcts.Games.Interfaces;
using System;
using System.Threading.Tasks;

namespace mcts.Tournaments.Bots
{
    public class RandomBot : IPlayer
    {
        private Random random = new Random();

        public Task<IMove> MakeMove(IGame game, int msPerMove)
        {
            var moves = game.GetLegalMoves();
            return Task.FromResult(moves[random.Next(moves.Count)]);
        }

        public void UseLogger(Logger logger)
        {
            return;
        }
    }
}

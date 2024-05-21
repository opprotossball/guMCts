using mcts.Games.Interfaces;
using mcts.Tournaments;

namespace mcts.Bot
{
    public interface IPlayer
    {
        public Task<IMove> MakeMove(IGame game, int msPerMove);
        public void UseLogger(Logger logger);
    }
}

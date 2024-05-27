using mcts.Games.Interfaces;
using mcts.Tournaments;
using System.Threading.Tasks;

namespace mcts.Bot
{
    public interface IPlayer
    {
        public Task<IMove> MakeMove(IGame game, int msPerMove);
        public void UseLogger(Logger logger);
    }
}

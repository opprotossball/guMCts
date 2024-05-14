using mcts.Games.Interfaces;

namespace mcts.Bot
{
    public interface IPlayer
    {
        public IMove MakeMove(IGame game);
    }
}

using mcts.Games.Interfaces;

namespace mcts.Bot
{
    public interface IMcts
    {
        public IMove MakeMove(IGame game);
    }
}

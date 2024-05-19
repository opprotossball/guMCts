using System.Diagnostics.Contracts;

namespace mcts.Games.Interfaces
{
    public interface IGame
    {
        public List<IMove> GetLegalMoves();
        public int PlayerToGo();
        public void UndoMove();
        public void MakeMove(IMove move);
        public GameResult Result(int playerId);
        //position hash
        public int GetHashCode();
        public IGame HistorylessCopy();
        public IMove PseudoRandomMove(Random random);
    }
}

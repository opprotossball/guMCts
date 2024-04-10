namespace mcts.Games.Interfaces
{
    public interface IGame
    {
        public List<IMove> GetLegalMoves();
        public int PlayerToGo();
        public void UndoMove();
        public void MakeMove(IMove move);
        public IGameResult Result(int playerId);
        //position hash
        public int GetHashCode();
        public void Restart();
        // public void UndoRandom();
    }
}

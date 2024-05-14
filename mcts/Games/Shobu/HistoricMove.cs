namespace mcts.Games.Shobu
{
    public class HistoricMove
    {
        public Move Move { get; }
        public bool WasPush { get; }
        public int PushedFrom { get; }

        public HistoricMove(Move move)
        {
            Move = Move.DeepCopy(move);
            WasPush = false;
        }

        public HistoricMove(Move move, int pushedFrom)
        {
            Move = Move.DeepCopy(move);
            WasPush = true;
            PushedFrom = pushedFrom;
        }
    }
}

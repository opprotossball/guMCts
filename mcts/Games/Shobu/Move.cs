using mcts.Games.Interfaces;

namespace mcts.Games.Shobu
{
    public class Move : IMove
    {
        public bool WhitePassiveBoard { get; set; }
        public bool AggressiveHomeBoard { get; set; }
        public int PassiveFrom { get; set; }
        public int AggressiveFrom { get; set; }
        public bool DoubleMove { get; set; }
        public Direction Direction { get; set; }

        public override string ToString()
        {
            String s = WhitePassiveBoard ? "passive: white" : "passive: black";
            s += " board, " + PassiveFrom.ToString() + " " + Direction.ToString();
            if (DoubleMove) s += " double";
            s += " | aggressive: " + AggressiveFrom.ToString();
            return s;
        }

        public static Move DeepCopy(Move move)
        {
            return new Move()
            {
                WhitePassiveBoard = move.WhitePassiveBoard,
                AggressiveHomeBoard = move.AggressiveHomeBoard,
                PassiveFrom = move.PassiveFrom,
                AggressiveFrom = move.AggressiveFrom,
                DoubleMove = move.DoubleMove,
                Direction = move.Direction
            };
        }
    }
}

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
            s += " | aggressive: ";
            s += AggressiveHomeBoard ? "home" : "opponents";
            s += "board, " + AggressiveFrom.ToString();
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

        override public int GetHashCode()
        {
            int hash = 23;
            hash = hash * 17 + WhitePassiveBoard.GetHashCode();
            hash = hash * 17 + AggressiveHomeBoard.GetHashCode();
            hash = hash * 17 + PassiveFrom.GetHashCode();
            hash = hash * 17 + AggressiveFrom.GetHashCode();
            hash = hash * 17 + DoubleMove.GetHashCode();
            hash = hash * 17 + Direction.GetHashCode();
            return hash;
        }
    }
}

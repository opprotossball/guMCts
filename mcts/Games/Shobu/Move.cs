using mcts.Games.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mcts.Games.Shobu
{
    public class Move : IMove
    {
        public bool WhitePassiveBoard { get; set; }
        public int PassiveFrom { get; set; }
        public int AggressiveFrom { get; set; }
        public bool DoubleMove { get; set; }
        public Direction Direction { get; set; }

        public override string ToString()
        {
            String s = WhitePassiveBoard ? "white" : "black";
            s += "board passive: " + PassiveFrom.ToString() + " " + Direction.ToString();
            if (DoubleMove) s += " double";
            s += "| aggressive: " + AggressiveFrom.ToString();
            return s;
        }
    }
}

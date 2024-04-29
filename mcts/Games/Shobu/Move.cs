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
        public bool WhitePassive { get; set; }
        public int PassiveFrom { get; set; }
        public int PassiveTo { get; set;}
        public int AggressiveFrom { get; set; }
    }
}

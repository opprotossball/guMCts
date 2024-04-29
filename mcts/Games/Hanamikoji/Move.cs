using mcts.Games.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mcts.Games.Hanamikoji
{
    public class Move : IMove
    {
        public bool Choosing { get; set; }
        public Actions Action { get; set; }
        public List<int>? Choices { get; set; }
    }
}

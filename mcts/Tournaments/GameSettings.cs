using System.Collections.Generic;

namespace mcts.Tournaments
{
    public class GameSettings
    {
        public int MsPerMove {  get; set; } 
        public int MaxMoves { get; set; }
        public bool UnfairTime { get; set; }
        public List<int> UnfairMsPerMove { get; set; }
    }
}

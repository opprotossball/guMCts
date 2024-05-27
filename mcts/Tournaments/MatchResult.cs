using System.Collections.Generic;

namespace mcts.Tournaments
{
    public class MatchResult
    {
        public List<double> Scores { get; set; } = new List<double>();
        public int MatchId = -1;
        public int Length { get; set; } = 0;
        public bool EndedSuccessfully { get; set; } = false;
        public bool Stalemate { get; set; } = false;
        public bool Timeout { get; set; } = false;
        public bool Error { get; set; } = false;
        public int DisqualifiedPlayer = -1;
    }
}

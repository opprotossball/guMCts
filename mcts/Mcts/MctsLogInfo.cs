namespace mcts.Mcts
{
    public class MctsLogInfo
    {
        public long TimePerMove { get; set; }
        public int SimulationDepthLimit { get; set; }
        public long TimeElapsed { get; set; }
        public int NumberOfNodes { get; set; }
        public int RootSimulations { get; set; }
        public int SimulationTimeouts { get; set; }
        public int MaxDepth { get; set; }
        public int FirstMovesConsidered { get; set; }
        public double MaxAverageScore { get; set; }
        public double MeanAverageScore { get; set; }
        public int MaxSimulations { get; set; }
    }
}

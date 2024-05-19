using mcts.Bot;
using mcts.Games.Interfaces;
using System.Diagnostics;

namespace mcts.Mcts
{
    public class Mcts : IPlayer
    {
        // add simulation policy
        private readonly Func<Node, double> selectionPolicy;
        // add time control
        private readonly int msPerMove;
        // make parameter
        private readonly int maxSimulationDepth = 300;
        private Stopwatch stopwatch;
        private Random random;
        private int simulationTimeOuts;
        private int stalemates;
        private long timeElapsed;

        public Mcts(Func<Node, double> selectionPolicy, int msPerMove) 
        { 
            this.selectionPolicy = selectionPolicy;
            this.msPerMove = msPerMove;
            random = new Random();
            stopwatch = new Stopwatch();
        }

        public IMove MakeMove(IGame game)
        {
            stopwatch.Restart();
            simulationTimeOuts = 0;
            stalemates = 0;
            timeElapsed = 0;
            Node root = new Node(game, null);
            while (stopwatch.ElapsedMilliseconds < msPerMove)
            {
                Iteration(root);
            }
            timeElapsed = stopwatch.ElapsedMilliseconds;
            stopwatch.Stop();
            return BestMove(root);
        }

        private IMove BestMove(Node root)
        {
            int maxSimulations = 0;
            int bestMove = 0;
            for (int i = 0; i < root.children.Length; i++) 
            {
                if (root.children[i] != null && root.children[i].simulations > maxSimulations)
                {
                    maxSimulations = root.children[i].simulations;
                    bestMove = i;
                }
            }
            // move elswhere
            Console.WriteLine(DebugLog(root));
            return root.position.GetLegalMoves()[bestMove];
        }


        private void Iteration(Node root)
        {
            Node node = root.Select(selectionPolicy);
            // Node newNode = node.Expand();
            IGame endingPosition = Simulate(node.position.HistorylessCopy(), root.activePlayer);
            node.Backpropagate(endingPosition);
        }

        // random playout - returns ending position for given player
        private IGame Simulate(IGame game, int playerId)
        {
            GameResult result = game.Result(playerId);
            int depth = 0;
            while (!result.IsOver)
            {
                if (depth++ > maxSimulationDepth)
                {
                    simulationTimeOuts++;
                    break;
                }
                //List<IMove> moves = game.GetLegalMoves();
                //if (moves.Count > 0) 
                //game.MakeMove(moves[random.Next(moves.Count)]);
                try
                {
                    IMove move = game.PseudoRandomMove(random);
                    game.MakeMove(move);
                }
                catch (InvalidOperationException e)
                {
                    stalemates += 1;
                    return game;
                }
                result = game.Result(playerId);
            }
            return game;
        }

        public string DebugLog(Node root)
        {
            string log = "\n---- MCTS log ----";
            log += $"\ntime per move [ms]: {msPerMove}";
            log += $"\nsimulation depth limit: {maxSimulationDepth}";

            log += $"\n\ntime elapsed [ms]: {timeElapsed}";
            int numOfNodes = root.SubtreeSize();
            log += $"\nnumber of nodes: {numOfNodes}";
            log += $"\nroot simulations: {root.simulations}";
            log += $"\nsimulation timeouts: {simulationTimeOuts}";
            int maxDepth = root.MaxDepth();
            log += $"\nmax depth: {maxDepth}";
            double maxAvgScore = int.MinValue;
            double avgAvgScore = 0;
            int maxSimulations = 0;
            int nodes = 0;
            foreach (Node node in root.children)
            {
                if (node == null) continue;
                maxAvgScore = Math.Max(maxAvgScore, node.scoreSum / node.simulations);
                maxSimulations = Math.Max(maxSimulations, node.simulations);
                avgAvgScore += node.scoreSum / node.simulations;
                nodes++;
            }
            avgAvgScore /= nodes;
            log += $"\n\nfirst moves considered: {nodes}";
            log += $"\nmax average score: {maxAvgScore}";
            log += $"\nmean average score: {Math.Round(avgAvgScore, 2)}";
            log += $"\nmax simulations: {maxSimulations}";
            log += "\n-----------------\n";
            return log;
        }
    }
}

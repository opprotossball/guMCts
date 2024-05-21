using mcts.Bot;
using mcts.Games.Interfaces;
using mcts.Tournaments;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Xml;

namespace mcts.Mcts
{
    public class Mcts : IPlayer
    {
        // add simulation policy
        private readonly Func<Node, double> selectionPolicy;
        // add time control
        private int msPerMove;
        // make parameter
        private readonly int maxSimulationDepth = 300;
        private Stopwatch stopwatch;
        private Random random;
        private int simulationTimeOuts;
        private int stalemates;
        private long timeElapsed;
        private Logger logger = new Logger();

        public Mcts()
        {
            selectionPolicy = SelectionPolicies.DefaultSelectionPolicy;
            random = new Random();
            stopwatch = new Stopwatch();
        }

        public Mcts(Func<Node, double> selectionPolicy, int msPerMove) 
        { 
            this.selectionPolicy = selectionPolicy;
            this.msPerMove = msPerMove;
            random = new Random();
            stopwatch = new Stopwatch();
        }

        public async Task<IMove> MakeMove(IGame game, int msPerMove)
        {
            stopwatch.Restart();
            this.msPerMove = msPerMove;
            simulationTimeOuts = 0;
            stalemates = 0;
            timeElapsed = 0;
            Node root = new Node(game, null);
            while (stopwatch.ElapsedMilliseconds < msPerMove * 0.975)
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
            logger.Log(DebugLog(root));
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
            var debugInfo = new MctsLogInfo
            {
                TimePerMove = msPerMove,
                SimulationDepthLimit = maxSimulationDepth,
                TimeElapsed = timeElapsed,
                NumberOfNodes = root.SubtreeSize(),
                RootSimulations = root.simulations,
                SimulationTimeouts = simulationTimeOuts,
                MaxDepth = root.MaxDepth(),
                MaxAverageScore = double.MinValue,
                MeanAverageScore = 0,
                MaxSimulations = 0,
                FirstMovesConsidered = 0
            };
            double totalAverageScore = 0;
            int nodes = 0;
            foreach (Node node in root.children)
            {
                if (node == null) continue;
                debugInfo.MaxAverageScore = Math.Max(debugInfo.MaxAverageScore, node.scoreSum / node.simulations);
                debugInfo.MaxSimulations = Math.Max(debugInfo.MaxSimulations, node.simulations);
                totalAverageScore += node.scoreSum / node.simulations;
                nodes++;
            }
            if (nodes > 0)
            {
                debugInfo.MeanAverageScore = Math.Round(totalAverageScore / nodes, 2);
                debugInfo.FirstMovesConsidered = nodes;
            }
            string logJson = JsonConvert.SerializeObject(debugInfo, Newtonsoft.Json.Formatting.Indented);
            return logJson;
        }

        public void UseLogger(Logger logger)
        {
            this.logger = logger;
        }
    }
}

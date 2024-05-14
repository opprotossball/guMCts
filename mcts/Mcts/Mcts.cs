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
        private Stopwatch stopwatch;
        private Random random;

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
            Node root = new Node(game, null);
            while (stopwatch.ElapsedMilliseconds < msPerMove)
            {
                Iteration(root);
            }
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
            return root.position.GetLegalMoves()[bestMove];
        }


        private void Iteration(Node root)
        {
            Node node = root.Select(selectionPolicy);
            Node newNode = node.Expand();
            IGame endingPosition = Simulate(newNode.position.HistorylessCopy(), root.activePlayer);
            newNode.Backpropagate(endingPosition);
        }

        // random playout - returns ending position for given player
        private IGame Simulate(IGame game, int playerId)
        {
            GameResult result = game.Result(playerId);
            while (!result.IsOver)
            {
                List<IMove> moves = game.GetLegalMoves();
                if (moves.Count > 0) game.MakeMove(moves[random.Next(moves.Count)]);
                else Console.WriteLine("xd");
                result = game.Result(playerId);
            }
            return game;
        }

    }
}

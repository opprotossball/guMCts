using mcts.Games.Interfaces;

namespace mcts.Mcts
{
    public class Node
    {
        public readonly int activePlayer;
        public IGame position;
        public readonly Node? parent;
        public Node[]? children;
        public double scoreSum;
        public int simulations;
        public bool IsLeaf 
        {
            get => children == null; 
        }
        public bool IsRoot
        {
            get => parent == null;
        }
        public bool FullyExplored { get; private set; }
        public int NextToExplore { get; private set; }
        public Node(IGame position, Node? parent)
        {
            this.position = position;
            this.parent = parent;
            FullyExplored = false;
            activePlayer = position.PlayerToGo();
        }

        // initializes children array, adds & returns Node coressponding to first valid move
        public Node Expand()
        {
            List<IMove> moves = position.GetLegalMoves();
            children = new Node[moves.Count];
            // add first node
            IGame newPosition = position.HistorylessCopy();
            newPosition.MakeMove(moves[0]);
            children[0] = new Node(newPosition, this);
            NextToExplore = 1;
            return children[0];
        }

        public Node Select(Func<Node, double> selectionPolicy)
        {
            // could be IsLeaf instead
            if (children == null) return this;
            // move caching will be needed
            List<IMove> moves = position.GetLegalMoves();
            if (!FullyExplored)
            {
                IGame newPosition = position.HistorylessCopy();
                newPosition.MakeMove(moves[NextToExplore]);
                children[NextToExplore] = new Node(newPosition, this);
                if (++NextToExplore == children.Length) FullyExplored = true;
                return children[NextToExplore].Select(selectionPolicy);
            }
            else
            {
                double maxVal = double.MinValue;
                int bestChild = 0;
                for (int i = 0; i < children.Length; i++)
                {
                    double newVal = selectionPolicy(children[i]);
                    if (newVal > maxVal)
                    {
                        maxVal = newVal;
                        bestChild = i;
                    }
                }
                return children[bestChild].Select(selectionPolicy);
            }
        }

        public void Backpropagate(IGame position)
        {
            simulations++;
            if (parent == null) return;
            GameResult result = position.Result(parent.activePlayer);
            if (result.IsOver)
            {
                scoreSum += result.Score;
            }
            parent.Backpropagate(position);
        }
    }
}

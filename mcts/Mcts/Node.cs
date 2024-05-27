using mcts.Games.Interfaces;
using System;
using System.Collections.Generic;

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
        private List<IMove>? _cachedMoves;
        private List<IMove> CachedMoves
        {
            get { EnsureMovesCached(); return _cachedMoves; }
            set { _cachedMoves = value; }
        }
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
            //List<IMove> moves = position.GetLegalMoves();
            List<IMove> moves = CachedMoves;
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
            if (position.IsOverForPlayers())
            {
                return this;
            }
            // could be IsLeaf instead
            if (children == null)
            {
                return Expand();
            }
            // move caching will be needed
            //List<IMove> moves = position.GetLegalMoves();
            List<IMove> moves = CachedMoves;
            if (!FullyExplored)
            {
                IGame newPosition = position.HistorylessCopy();
                newPosition.MakeMove(moves[NextToExplore]);
                children[NextToExplore] = new Node(newPosition, this);
                if (++NextToExplore == children.Length) FullyExplored = true;
                return children[NextToExplore - 1].Select(selectionPolicy);
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

        public int SubtreeSize()
        {
            int result = 0; 
            if (children == null) return result;
            foreach (Node node in children)
            {
                if (node == null) continue;
                result += node.SubtreeSize();
            }

            return result + 1;
        }

        public int MaxDepth()
        {
            if (children == null)
                return 1;
            int maxDepth = children[0].MaxDepth();
            for (int i = 1; i < children.Length; i++)
            {
                if (children[i] == null) continue;
                int childDepth = children[i].MaxDepth();
                if (childDepth > maxDepth)
                    maxDepth = childDepth;
            }
            return maxDepth + 1;
        }

        private void EnsureMovesCached()
        {
            if (_cachedMoves != null) return;
            _cachedMoves = position.GetLegalMoves();
        }
    }
}

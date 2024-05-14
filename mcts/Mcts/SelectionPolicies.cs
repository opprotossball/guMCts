namespace mcts.Mcts
{
    public class SelectionPolicies
    {
        public static double DefaultSelectionPolicy(Node node)
        {
            if (node.parent == null) return 0.0;
            double c = Math.Sqrt(2);
            return node.scoreSum / node.simulations + 
                c * Math.Sqrt(Math.Log(node.parent.simulations) / node.simulations);
        }
    }
}

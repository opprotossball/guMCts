namespace mcts.Games.Interfaces
{
    public interface IGameResult
    {
        bool IsOver { get; }
        int Score { get; }
    }
}

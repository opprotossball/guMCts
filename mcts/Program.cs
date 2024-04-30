using mcts.Games.Interfaces;
using mcts.Games.Shobu;

Shobu game = new Shobu();
foreach (Move m in game.ShortMovesForBoard(true))
{
    Console.WriteLine(m);
}
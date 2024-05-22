using mcts.Analysis.ShobuAnalysis;
using mcts.Games.Interfaces;
using mcts.Games.Shobu;
using mcts.Mcts;
using mcts.Tournaments;
using mcts.Tournaments.Bots;

//Shobu game = new Shobu();
//Mcts mcts = new Mcts(SelectionPolicies.DefaultSelectionPolicy, 10000);
//Move move = (Move)await mcts.MakeMove(game, 10000);
//Console.WriteLine(move);

//ShobuAnalysis.SaveLengths(ShobuAnalysis.GameLength(500), "C:\\Users\\janst\\source\\studia\\dypl\\Analysis\\shobu_lengths_2.csv");

//ShobuAnalysis.SaveToCsv(ShobuAnalysis.WinRate(500), "C:\\Users\\janst\\source\\studia\\dypl\\Analysis\\shobu_wins.csv");

//ShobuAnalysis.SaveToCsv(ShobuAnalysis.BranchingFactor(500), "C:\\Users\\janst\\source\\studia\\dypl\\Analysis\\shobu_branching.csv");

//ShobuAnalysis.VersionConsistenceTest();

//ShobuAnalysis.PseudoRandomGame(500);

Type gameType = typeof(Shobu);
List<Type> playerTypes = new List<Type>() { typeof(Mcts), typeof(Mcts) };
GameSettings settings = new GameSettings()
{
    msPerMove = 30000,
    maxMoves = 300,
};
int nMatches = 8;
await Arena.Tournament(gameType, playerTypes, nMatches, settings, "C:\\Users\\janst\\OneDrive\\Dokumenty\\Studia\\dypl\\both");

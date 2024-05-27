//ShobuAnalysis.SaveLengths(ShobuAnalysis.GameLength(500), "C:\\Users\\janst\\source\\studia\\dypl\\Analysis\\shobu_lengths_2.csv");

//ShobuAnalysis.SaveToCsv(ShobuAnalysis.WinRate(500), "C:\\Users\\janst\\source\\studia\\dypl\\Analysis\\shobu_wins.csv");

//ShobuAnalysis.SaveToCsv(ShobuAnalysis.BranchingFactor(500), "C:\\Users\\janst\\source\\studia\\dypl\\Analysis\\shobu_branching.csv");

//ShobuAnalysis.VersionConsistenceTest();

//ShobuAnalysis.PseudoRandomGame(500);

using mcts.Games.Shobu;
using mcts.Mcts;
using mcts.Tournaments;

Type gameType = typeof(Shobu);
List<Type> playerTypes = new List<Type>() { typeof(Mcts), typeof(Mcts) };
GameSettings settings = new GameSettings()
{
    //MsPerMove = 30000,
    MaxMoves = 300,
    UnfairTime = true,
    UnfairMsPerMove = new List<int>() { 7500, 5000 }
};
int nMatches = 300;
await Arena.Tournament(gameType, playerTypes, nMatches, settings, "C:\\Users\\janst\\OneDrive\\Dokumenty\\Studia\\dypl\\shobuUnfair");
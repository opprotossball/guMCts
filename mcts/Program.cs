using mcts.Analysis.ShobuAnalysis;
using mcts.Games.Interfaces;
using mcts.Games.Shobu;
using mcts.Mcts;

Shobu game = new Shobu();
Mcts mcts = new Mcts(SelectionPolicies.DefaultSelectionPolicy, 10000);
Move move = (Move)mcts.MakeMove(game);
Console.WriteLine(move);

//ShobuAnalysis.SaveLengths(ShobuAnalysis.GameLength(500), "C:\\Users\\janst\\source\\studia\\dypl\\Analysis\\shobu_lengths_2.csv");

//ShobuAnalysis.SaveToCsv(ShobuAnalysis.WinRate(500), "C:\\Users\\janst\\source\\studia\\dypl\\Analysis\\shobu_wins.csv");

//ShobuAnalysis.SaveToCsv(ShobuAnalysis.BranchingFactor(500), "C:\\Users\\janst\\source\\studia\\dypl\\Analysis\\shobu_branching.csv");

//ShobuAnalysis.VersionConsistenceTest();

//ShobuAnalysis.PseudoRandomGame(500);
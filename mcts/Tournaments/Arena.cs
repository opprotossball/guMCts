using mcts.Bot;
using mcts.Games.Interfaces;
using Newtonsoft.Json;

namespace mcts.Tournaments
{
    public class Arena
    {
        public static async Task Tournament(Type gameType, List<Type> playerTypes, int nMatches, GameSettings gameSettings, string logDir)
        {
            Logger logger = new Logger(logDir + "\\tournamentLog.txt");
            int matchId = -1;
            List<Task> matches = new List<Task>();
            while (++matchId < nMatches)
            {
                int currentMatch = matchId;
                Task match = Task.Run(() => PlayMatch(currentMatch, gameType, playerTypes, gameSettings, logDir, logger));
                matches.Add(match);
            }
            await Task.WhenAll(matches);
        }

        private static async Task PlayMatch(int matchId, Type gameType, List<Type> playerTypes, GameSettings gameSettings, string logDir, Logger logger)
        {
            Console.WriteLine($"Match {matchId} started");
            List<IPlayer> players = new List<IPlayer>();
            foreach (Type playerType in playerTypes)
            {
                players.Add((IPlayer)Activator.CreateInstance(playerType));
            }
            IGame game = (IGame)Activator.CreateInstance(gameType);
            string matchDir = logDir + $"\\match{matchId}";
            Directory.CreateDirectory(matchDir);
            MatchManager match = new MatchManager(players, game, gameSettings, matchDir);
            MatchResult result = await match.Play();
            result.MatchId = matchId;
            logger.Log(JsonConvert.SerializeObject(result, Formatting.Indented));
            Console.WriteLine($"Match {matchId} ended");
        }
    }
}

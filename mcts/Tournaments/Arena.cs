using mcts.Bot;
using mcts.Games.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace mcts.Tournaments
{
    public class Arena
    {
        public static async Task Tournament(Type gameType, List<Type> playerTypes, int nMatches, GameSettings gameSettings, string logDir)
        {
            Logger logger = new Logger(logDir + "\\tournamentLog.txt");

            string startDate = String.Format("{0:u}", DateTime.Now.ToString());
            logger.Log($"Started {startDate}");
            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss"));

            int maxConcurrent = Environment.ProcessorCount;
            Console.WriteLine($"{maxConcurrent} processors found");
            SemaphoreSlim semaphoreSlim = new SemaphoreSlim(maxConcurrent);
            int matchId = -1;
            List<Task> matches = new List<Task>();

            while (++matchId < nMatches)
            {
                int currentMatch = matchId;
                await semaphoreSlim.WaitAsync();
                Task match = Task.Run(async () =>
                {
                    try
                    {
                        await PlayMatch(currentMatch, gameType, playerTypes, gameSettings, logDir, logger);
                    }
                    finally
                    {
                        semaphoreSlim.Release();
                    }
                    
                });
                matches.Add(match);
            }

            await Task.WhenAll(matches);

            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss"));
            string endDate = String.Format("{0:u}", DateTime.Now.ToString());
            logger.Log($"Started {endDate}");
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

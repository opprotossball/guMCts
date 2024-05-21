using mcts.Bot;
using mcts.Games.Interfaces;
using Newtonsoft.Json;
using System.Diagnostics;

namespace mcts.Tournaments
{
    public class MatchManager
    {
        private List<IPlayer> _players;
        private IGame _game;
        private GameSettings _settings;
        public bool HasEnded { get; private set; }
        private int _movesMade;
        private bool _errorOccurred;
        private bool _timeoutOccured;
        private readonly string _logDir;
        private readonly Logger _matchLogger;

        public MatchManager(List<IPlayer> players, IGame game, GameSettings gameSettings, string logDir) 
        {
            HasEnded = false;
            _players = players;
            _game = game;
            _settings = gameSettings;
            _logDir = logDir;
            _movesMade = 0;
            _errorOccurred = false;
            _matchLogger = new Logger(logDir + $"\\matchlog.txt");
        }

        public async Task<MatchResult> Play()
        {
            SetupLogging();
            while (!HasEnded)
            {
                if (_movesMade > _settings.maxMoves)
                {
                    return new MatchResult()
                    {
                        Stalemate = true,
                        Length = _movesMade,
                        EndedSuccessfully = true,
                    };
                }
                await Move();
            }
            if (_errorOccurred || _timeoutOccured)
            {
                return new MatchResult()
                {
                    Error = _errorOccurred,
                    Timeout = _timeoutOccured,
                    DisqualifiedPlayer = _game.PlayerToGo(),
                    EndedSuccessfully = false,
                    Length = _movesMade,
                };
            }
            List<double> scores = new List<double>();
            for (int player = 0; player < _players.Count; player++)
            {
                scores.Add(_game.Result(player).Score);
            }
            return new MatchResult()
            {
                EndedSuccessfully = true,
                Length = _movesMade,
                Scores = scores,
            };
        }

        private void SetupLogging()
        {
            int i = 0;
            foreach (var player in _players)
            {
                player.UseLogger(new Logger(_logDir + $"\\player{i++}log.txt"));
            }
        }

        private async Task Move() 
        { 
            if (_game.IsOverForPlayers())
            {
                _matchLogger.Log($"Match has ended successfully in {_movesMade} moves");
                HasEnded = true;
                return;
            }
            int activePlayer = _game.PlayerToGo();
            if (activePlayer >= _players.Count) 
            {
                throw new Exception("Not enough players");
            }
            IGame position = _game.HistorylessCopy();
            Task moveTime = Task.Delay(_settings.msPerMove);
            Stopwatch stopwatch = Stopwatch.StartNew();
            Task<IMove> choosingMove = _players[activePlayer].MakeMove(position, _settings.msPerMove);
            Task finished = await Task.WhenAny(moveTime, choosingMove);
            stopwatch.Stop();
            if (finished == moveTime)
            {
                _matchLogger.Log($"Player {_game.PlayerToGo()} timed out");
                HasEnded = true;
                _timeoutOccured = true;
                return;
            }
            try
            {
                IMove move = await choosingMove;
                LogMove(move, stopwatch.ElapsedMilliseconds);
                _game.MakeMove(move);
            }
            catch (Exception ex)
            {
                _matchLogger.Log($"Player {_game.PlayerToGo()}: {ex.Message}");
                _errorOccurred = true;
                HasEnded = true;
                return;
            }
            _movesMade++;
        }

        private void LogMove(IMove move, long timeUsed)
        {
            MoveLog log = new MoveLog()
            {
                Player = _game.PlayerToGo(),
                Move = move.ToString(),
                TimeUsed = timeUsed,
            };
            string logJson = JsonConvert.SerializeObject(log, Formatting.Indented);
            _matchLogger.Log(logJson);
        }

    }
}

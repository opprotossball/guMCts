using mcts.Games.Interfaces;

namespace mcts.Games.Shobu
{
    public class Shobu : IGame
    {
        private static readonly int NumOfSquares = 64;
        private static readonly int BorderTile = -255;
        private static readonly int NumOfBoards = 4;
        private int winner;
        private bool whiteToGo;
        public int[] whitePlayerWhiteBoard = new int[NumOfSquares];
        public int[] whitePlayerBlackBoard = new int[NumOfSquares];
        public int[] blackPlayerWhiteBoard = new int[NumOfSquares];
        public int[] blackPlayerBlackBoard = new int[NumOfSquares];
        public List<int>[] whitePieces = new List<int>[NumOfBoards];
        public List<int>[] blackPieces = new List<int>[NumOfBoards];
        private Stack<HistoricMove> history;

        public Shobu()
        {
            for (int i = 0; i < NumOfBoards; i++)
            {
                whitePieces[i] = new List<int>();
                blackPieces[i] = new List<int>();
            }
            for (int whiteHome = 0; whiteHome < 2; whiteHome++)
                for (int whiteBoard = 0; whiteBoard < 2; whiteBoard++)
                    ResetBoard(whiteHome == 1, whiteBoard == 1);
            history = new Stack<HistoricMove>();
        }

        public List<int> PieceList(bool whitePlayer, bool whiteBoard, bool whiteHome)
        {
            int board = 0;
            if (whiteBoard) board++;
            if (whiteHome) board += 2;
            return whitePlayer ? whitePieces[board] : blackPieces[board];
        }

        private Shobu(Shobu toCopy)
        {
            whiteToGo = toCopy.whiteToGo;
            winner = toCopy.winner;
            for (int i = 0; i < NumOfBoards; i++)
            {
                whitePieces[i] = new List<int>();
                blackPieces[i] = new List<int>();
            }
            for (int whiteHome = 0; whiteHome < 2; whiteHome++)
            {
                for (int whiteBoard = 0; whiteBoard < 2; whiteBoard++)
                {
                    int[] board = BoardForPlayer(whiteHome == 1, whiteBoard == 1);
                    int[] toCopyBoard = toCopy.BoardForPlayer(whiteHome == 1, whiteBoard == 1);
                    for (int i = 0; i < board.Length; i++)
                    {
                        board[i] = toCopyBoard[i];
                    }
                    for (int whitePlayer = 0; whitePlayer < 2; whitePlayer++)
                    {
                        List<int> pieceList = PieceList(whitePlayer == 1, whiteBoard == 1, whiteHome == 1);
                        List<int> toCopyPieceList = toCopy.PieceList(whitePlayer == 1, whiteBoard == 1, whiteHome == 1);
                        foreach (int piece in toCopyPieceList)
                        {
                            pieceList.Add(piece);
                        }
                    }
                }
            }
            history = new Stack<HistoricMove>();
        }

        public IGame HistorylessCopy() => new Shobu(this);

        public List<IMove> GetLegalMoves()
        {
            List<IMove> res = new List<IMove>();
            for (int whiteBoard = 0; whiteBoard < 2; whiteBoard++)
                for (int aggresiveHome = 0; aggresiveHome < 2; aggresiveHome++)
                    for (int doubleMoves = 0; doubleMoves < 2; doubleMoves++)
                        foreach (Move m in MovesForBoard(whiteBoard == 1, aggresiveHome == 1, doubleMoves == 1))
                            res.Add(m);
            return res;
        }

        private void UpdatePieceList(bool whitePlayer, bool whiteBoard, bool whiteHome, int from, int to)
        {
            List<int> pieceList = PieceList(whitePlayer, whiteBoard, whiteHome);
            for (int i = 0; i < pieceList.Count; i++)
            {
                if (pieceList[i] == from)
                {
                    pieceList[i] = to;
                    return;
                }
            }
        }

        private void RemoveFromPieceList(bool whitePlayer, bool whiteBoard, bool whiteHome, int from)
        {
            List<int> pieceList = PieceList(whitePlayer, whiteBoard, whiteHome);
            pieceList.Remove(from);
        }

        public void MakeMove(IMove move)
        {
            if (winner != 0) throw new InvalidOperationException("Game has ended!");
            if (!(move is Move)) throw new ArgumentException(nameof(move).ToString() + " Is invalid move type!");
            Move m = (Move)move;
            // choose boards
            int[] passiveBoard = BoardForPlayer(whiteToGo, m.WhitePassiveBoard);
            int[] aggressiveBoard = BoardForPlayer(whiteToGo == m.AggressiveHomeBoard, !m.WhitePassiveBoard);
            // validate move
            if (!CanBePlayed(passiveBoard, m.PassiveFrom, (int)m.Direction, m.DoubleMove, false))
            {
                throw new ArgumentException("Passive move cannot be played!");
            }
            if (!CanBePlayed(aggressiveBoard, m.AggressiveFrom, (int)m.Direction, m.DoubleMove, true))
            {
                throw new ArgumentException("Aggressive move cannot be played!");
            }
            int dir = (int)m.Direction;
            int diff = m.DoubleMove ? 2 * dir : dir;
            // make passive move
            passiveBoard[m.PassiveFrom + diff] = passiveBoard[m.PassiveFrom];
            passiveBoard[m.PassiveFrom] = 0;
            UpdatePieceList(whiteToGo, m.WhitePassiveBoard, whiteToGo, m.PassiveFrom, m.PassiveFrom + diff);
            // push
            int pushedFrom = -1;
            if (aggressiveBoard[m.AggressiveFrom + diff] != 0)
            {
                pushedFrom = m.AggressiveFrom + diff;
            }
            else if (m.DoubleMove && aggressiveBoard[m.AggressiveFrom + dir] != 0)
            {
                pushedFrom = m.AggressiveFrom + dir;
            }
            if (pushedFrom != -1)
            {
                int pushedTo = m.AggressiveFrom + diff + dir;
                int pushedPiece = aggressiveBoard[pushedFrom];
                aggressiveBoard[pushedFrom] = 0;
                if (aggressiveBoard[pushedTo] == BorderTile)
                {
                    CheckWinner(aggressiveBoard);
                    RemoveFromPieceList(!whiteToGo, !m.WhitePassiveBoard, m.AggressiveHomeBoard == whiteToGo, pushedFrom);
                }
                else
                {
                    aggressiveBoard[pushedTo] = pushedPiece;
                    UpdatePieceList(!whiteToGo, !m.WhitePassiveBoard, m.AggressiveHomeBoard == whiteToGo, pushedFrom, pushedTo);
                }
            }
            // make aggressive move
            aggressiveBoard[m.AggressiveFrom + diff] = aggressiveBoard[m.AggressiveFrom];
            aggressiveBoard[m.AggressiveFrom] = 0;
            UpdatePieceList(whiteToGo, !m.WhitePassiveBoard, m.AggressiveHomeBoard == whiteToGo, m.AggressiveFrom, m.AggressiveFrom + diff);
            // change active player
            whiteToGo = !whiteToGo;
            // add to history
            HistoricMove historic;
            if (IsValidTile(pushedFrom))
            {
                historic = new HistoricMove(m, pushedFrom);
            }
            else
            {
                historic = new HistoricMove(m);
            }
            history.Push(historic);
        }

        public int PlayerToGo() => whiteToGo ? 1 : 0;

        public GameResult Result(int playerId)
        {
            if (playerId != 0 && playerId != 1)
            {
                throw new ArgumentException("Player ID must be 0 or 1");
            }
            int score = 0;
            int player = playerId == 0 ? -1 : 1;
            if (winner != 0)
            {
                score = player == winner ? 1 : -1;
            }
            return new GameResult()
            {
                IsOver = winner != 0,
                Score = score,
            };
        }

        public void UndoMove()
        {
            throw new NotImplementedException();
        }

        private void CheckWinner(int[] board)
        {
            int whiteStones = 0;
            int blackStones = 0;
            foreach (int i in BoardIterator())
            {
                if (board[i] == -1) blackStones++;
                if (board[i] == 1) whiteStones++;

            }
            if (whiteStones == 0) winner = -1;
            if (blackStones == 0) winner = 1;
        }

        private bool IsValidTile(int tile) => (tile > 15 && tile < 47 && tile % 8 > 1 && tile % 8 < 6);

        private void ResetBoard(bool whiteHome, bool whiteBoard)
        {
            int[] board = BoardForPlayer(whiteHome, whiteBoard);
            List<int> whitePieces = PieceList(true, whiteBoard, whiteHome);
            List<int> blackPieces = PieceList(false, whiteBoard, whiteHome);
            whitePieces.Clear();
            blackPieces.Clear();
            for (int i = 0; i < board.Length; i++)
            {
                if (!IsValidTile(i)) board[i] = BorderTile;
                else if (i / 8 == 2)
                {
                    board[i] = 1;
                    whitePieces.Add(i);
                }
                else if (i / 8 == 5)
                {
                    board[i] = -1;
                    blackPieces.Add(i);
                }
                else board[i] = 0;
            }
        }

        public IEnumerable<int> BoardIterator()
        {
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    yield return j + 18 + (8 * i);
        }

        private IEnumerable<int> DirectionIterator() 
        {
            yield return -8;
            yield return -7;
            yield return 1;
            yield return 9;
            yield return 8;
            yield return 7;
            yield return -1;
            yield return -9;
        }

        private IEnumerable<int> PieceIterator(bool whitePlayer, bool whiteBoard, bool whiteHome)
        {
            foreach (int tile in PieceList(whitePlayer, whiteBoard, whiteHome))
                yield return tile;
        }

        private bool EmptyOrBorder(int tile) => tile == 0 || tile == BorderTile;

        private bool CanBePlayed(int[] board, int from, int dir, bool doubleMove, bool aggressive)
        {
            int to = doubleMove ? from + (2 * dir) : from + dir;
            // goes out of board
            if (board[to] == BorderTile) return false;
            // blocked by friendly
            if (board[to] == board[from]) return false;
            if (!aggressive)
            {
                // non-aggressive blocked by enemy
                if (board[to] != 0) return false;
                // jumps over piece
                if (doubleMove && board[from + dir] != 0) return false;
            }
            else
            {
                if (doubleMove)
                {
                    // jumps over friendly
                    if (board[from + dir] == board[from]) return false;
                    // double move push blocked - more than 1 stone in path
                    int pieceCount = 0;
                    for (int i = 1; i < 4; i++)
                    {
                        if (!EmptyOrBorder(board[from + i * dir])) pieceCount++;
                    }
                    if (pieceCount > 1) return false;
                }
                else
                {
                    // single move push blocked
                    if (board[to] != 0 && !EmptyOrBorder(board[to + dir])) return false;
                }
            }
            return true;
        }

        public IEnumerable<Move> MovesForBoard(bool whitePassiveBoard, bool homeAggressiveBoard, bool doubleMoves)
        {
            int[] passiveBoard = BoardForPlayer(whiteToGo, whitePassiveBoard);
            int[] aggressiveBoard = BoardForPlayer(whiteToGo == homeAggressiveBoard, !whitePassiveBoard);
            // for each piece
            foreach (int passiveFrom in PieceIterator(whiteToGo, whitePassiveBoard, whiteToGo))
            {
                // for each direction
                foreach (int dir in DirectionIterator())
                {
                    // pasive move can't be played
                    if (!CanBePlayed(passiveBoard, passiveFrom, dir, doubleMoves, false)) continue;
                    // for each piece on opponents board 
                    foreach (int aggressiveFrom in PieceIterator(whiteToGo, !whitePassiveBoard, homeAggressiveBoard == whiteToGo))
                    {
                        // aggressive move can be played
                        if (CanBePlayed(aggressiveBoard, aggressiveFrom, dir, doubleMoves, true))
                        {
                            yield return new Move()
                            {
                                WhitePassiveBoard = whitePassiveBoard,
                                AggressiveHomeBoard = homeAggressiveBoard,
                                PassiveFrom = passiveFrom,
                                AggressiveFrom = aggressiveFrom,
                                DoubleMove = doubleMoves,
                                Direction = (Direction)dir
                            };
                        }
                    }
                }
            }
        }

        private int[] BoardForPlayer(bool whiteHome, bool whiteBoard)
        {
            if (whiteHome && whiteBoard) return whitePlayerWhiteBoard;
            if (whiteHome && !whiteBoard) return whitePlayerBlackBoard;
            if (!whiteHome && whiteBoard) return blackPlayerWhiteBoard;
            return blackPlayerBlackBoard;
        }

        public int[] BoardArrayForPlayer(bool whitePlayer, bool whiteBoard)
        {
            int[] res = new int[16];
            int i = 0;
            int[] board = BoardForPlayer(whitePlayer, whiteBoard);
            foreach (int j in BoardIterator()) res[i++] = board[j];
            return res;
        }

        public IMove PseudoRandomMove(Random random)
        {
            int tries = 100;
            while (tries-- > 0)
            {
                bool whiteBoard = random.Next(2) == 1;
                bool aggressiveHome = random.Next(2) == 1;
                bool doubleMoves = random.Next(2) == 1;

                var passivePieces = PieceList(whiteToGo, whiteBoard, whiteToGo);
                if (passivePieces.Count == 0) continue;
                int passiveFrom = passivePieces[random.Next(passivePieces.Count)];

                var aggressivePieces = PieceList(whiteToGo, !whiteBoard, aggressiveHome == whiteToGo);
                if (aggressivePieces.Count == 0) continue;
                int aggressiveFrom = aggressivePieces[random.Next(aggressivePieces.Count)];

                var dirs = DirectionIterator().ToList();
                Direction direction = (Direction)random.Next(dirs.Count);

                int[] passiveBoard = BoardForPlayer(whiteToGo, whiteBoard);
                int[] aggressiveBoard = BoardForPlayer(whiteToGo == aggressiveHome, !whiteBoard);

                if (!CanBePlayed(passiveBoard, passiveFrom, (int)direction, doubleMoves, false)) continue;
                
                if (!CanBePlayed(aggressiveBoard, aggressiveFrom, (int)direction, doubleMoves, true)) continue;

                return new Move()
                {
                    Direction = direction,
                    AggressiveFrom = aggressiveFrom,
                    PassiveFrom = passiveFrom,
                    WhitePassiveBoard = whiteBoard,
                    AggressiveHomeBoard = aggressiveHome,
                    DoubleMove = doubleMoves
                };
            }
            List <IMove> moves = GetLegalMoves();
            if (moves.Count == 0)
            {
                throw new InvalidOperationException("No moves possible");
            }
            return moves[random.Next(moves.Count)];
        }

        public bool IsOverForPlayers() => winner != 0;
        public static int ArrayToBoardTile(int tile) => 18 + (tile % 4) + 8 * (tile / 4);
        public static int BoardToArrayTile(int tile) => tile - 18 - (4 * ((tile - 18) / 8));
    }
}

using mcts.Games.Interfaces;

namespace mcts.Games.Shobu
{
    public class Shobu : IGame
    {
        private static readonly int NumOfSquares = 64;
        private static readonly int BorderTile = -255;
        private int winner;
        private bool whiteToGo;
        public int[] whitePlayerWhiteBoard = new int[NumOfSquares];
        public int[] whitePlayerBlackBoard = new int[NumOfSquares];
        public int[] blackPlayerWhiteBoard = new int[NumOfSquares];
        public int[] blackPlayerBlackBoard = new int[NumOfSquares];
        private Stack<HistoricMove> history;

        public Shobu()
        {
            ResetBoard(whitePlayerWhiteBoard);
            ResetBoard(whitePlayerBlackBoard);
            ResetBoard(blackPlayerWhiteBoard);
            ResetBoard(blackPlayerBlackBoard);
            history = new Stack<HistoricMove>();
        }

        private Shobu(Shobu toCopy)
        {
            whiteToGo = toCopy.whiteToGo;
            winner = toCopy.winner;
            for (int whitePlayer = 0; whitePlayer < 2; whitePlayer++)
            {
                for (int whiteBoard = 0; whiteBoard < 2; whiteBoard++)
                {
                    int[] board = BoardForPlayer(whitePlayer == 1, whiteBoard == 1);
                    int[] toCopyBoard = toCopy.BoardForPlayer(whitePlayer == 1, whiteBoard == 1);
                    for (int i = 0; i < board.Length; i++)
                    {
                        board[i] = toCopyBoard[i];
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
                }
                else
                {
                    aggressiveBoard[pushedTo] = pushedPiece;
                }
            }
            // make aggressive move
            aggressiveBoard[m.AggressiveFrom + diff] = aggressiveBoard[m.AggressiveFrom];
            aggressiveBoard[m.AggressiveFrom] = 0;
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

        private void ResetBoard(int[] board)
        {
            for (int i = 0; i < board.Length; i++)
            {
                if (!IsValidTile(i)) board[i] = BorderTile;
                else if (i / 8 == 2) board[i] = 1;
                else if (i / 8 == 5) board[i] = -1;
                else board[i] = 0;
            }
        }

        public IEnumerable<int> BoardIterator()
        {
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    yield return j + 18 + (8 * i);
        }

        private IEnumerable<int> PieceIterator(int[] board, bool whitePlayer)
        {
            int piece = whitePlayer ? 1 : -1;
            foreach (int i in BoardIterator())
                if (board[i] == piece) yield return i;
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
            foreach (int passiveFrom in PieceIterator(passiveBoard, whiteToGo))
            {
                // for each direction
                foreach (int dir in Enum.GetValues(typeof(Direction)))
                {
                    // pasive move can't be played
                    if (!CanBePlayed(passiveBoard, passiveFrom, dir, doubleMoves, false)) continue;
                    // for each piece on opponents board 
                    foreach (int aggressiveFrom in PieceIterator(aggressiveBoard, whiteToGo))
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

        private int[] BoardForPlayer(bool whitePlayer, bool whiteBoard)
        {
            if (whitePlayer && whiteBoard) return whitePlayerWhiteBoard;
            if (whitePlayer && !whiteBoard) return whitePlayerBlackBoard;
            if (!whitePlayer && whiteBoard) return blackPlayerWhiteBoard;
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

        public static int ArrayToBoardTile(int tile) => 18 + (tile % 4) + 8 * (tile / 4);
        public static int BoardToArrayTile(int tile) => tile - 18 - (4 * ((tile - 18) / 8));
    }
}

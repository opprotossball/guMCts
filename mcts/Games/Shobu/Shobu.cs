using mcts.Games.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mcts.Games.Shobu
{
    public class Shobu : IGame
    {
        private bool whiteToGo;
        private static readonly int nSquares = 16;
        private int[] white_player_white_board = new int[nSquares];
        public int[] white_player_black_board = new int[nSquares];
        private int[] black_player_white_board = new int[nSquares];
        private int[] black_player_black_board = new int[nSquares];

        public Shobu()
        {
            ResetBoard(white_player_white_board);
            ResetBoard(white_player_black_board);
            ResetBoard(black_player_white_board);
            ResetBoard(black_player_black_board);
        }

        //private void ResetBoard(int[] board)
        //{
        //    for (int i = 0; i < board.Length; i++)
        //    {
        //        if (!ValidTile(i)) board[i] = 0;
        //        else if (i / 8 == 2) board[i] = 1;
        //        else if (i / 8 == 5) board[i] = -1;
        //        else board[i] = 0;
        //    }
        //}

        private void ResetBoard(int[] board)
        {
            if (board.Length != 16)
            {
                throw new ArgumentException("");
            }
            for (int i = 0; i < 16; i++)
            {
                if (i < 4) board[i] = 1;
                else if (i > 11) board[i] = -1;
                else board[i] = 0;
            }
        }

        private IEnumerable<int> BoardIterator()
        {
            for (int i = 0; i < 16; i++) yield return i;
        }

        private IEnumerable<int> PieceIterator(int[] board, bool whitePlayer)
        {
            int piece = whitePlayer ? 1 : -1;
            foreach (int i in BoardIterator())
            {
                if (board[i] == piece) yield return i;
            }
        }


        private int[] BoardForPlayer(bool whitePlayer, bool whiteBoard)
        {
           if (whitePlayer && whiteBoard) return white_player_white_board;
           if (whitePlayer && !whiteBoard) return white_player_black_board;
           if (!whitePlayer && whiteBoard) return black_player_white_board;
           return black_player_black_board;
        }
        private bool ValidExtendedTile(int tile) => (tile > 15 && tile < 47 && tile % 8 > 1 && tile % 8 < 6);
                 
        private int ExtendedCoordinate(int tile) => 18 + (tile % 4) + 8 * (tile / 4);

        private int ReducedCoordinate(int tile) => tile - 18 - (4 * ((tile - 18) / 8));

        public IEnumerable<Move> ShortMovesForBoard(bool whiteBoard)
        {
            int[] passvieBoard = BoardForPlayer(whiteToGo, whiteBoard);
            int[] agressvieBoard = BoardForPlayer(!whiteToGo, !whiteBoard);
            // for each piece 
            foreach (int piece in PieceIterator(passvieBoard, whiteToGo))
            {
                int fromExt = ExtendedCoordinate(piece);
                // for each direction
                foreach (int dir in Enum.GetValues(typeof(Direction)))
                {
                    int toExt = fromExt + dir;
                    if (!ValidExtendedTile(toExt)) continue;
                    int to = ReducedCoordinate(toExt);
                    if (passvieBoard[to] != 0) continue;
                    // for each piece on other board
                    foreach (int aggresivePiece in PieceIterator(agressvieBoard, whiteToGo))
                    {
                        int aggresiveFromExt = ExtendedCoordinate(aggresivePiece);
                        int aggresiveToExt = aggresiveFromExt + dir;
                        if (!ValidExtendedTile(aggresiveToExt)) continue;
                        int aggresiveTo = ReducedCoordinate(aggresiveToExt);
                        // move blocked by friendly stone
                        if (agressvieBoard[aggresiveTo] == agressvieBoard[aggresivePiece]) continue;
                        // move blocked by enemy which can't be pushed
                        if (agressvieBoard[aggresiveTo] != 0 && !CanBePushed(agressvieBoard, aggresiveTo, dir)) continue;
                        yield return new Move()
                        {
                            WhitePassiveBoard = whiteBoard,
                            DoubleMove = false,
                            PassiveFrom = piece,
                            AggressiveFrom = aggresivePiece,
                            Direction = (Direction)dir
                        };
                    }
                }
            }
        }

        private bool CanBePushed(int[] board, int piece, int dir)
        {
            int fromExt = ExtendedCoordinate(piece);
            int toExt = fromExt + dir;
            if (!ValidExtendedTile(toExt)) return true;
            if (board[ReducedCoordinate(toExt)] == 0) return true;
            return false;
        }

        public List<IMove> GetLegalMoves()
        {
            throw new NotImplementedException();
        }

        public void MakeMove(IMove move)
        {
            throw new NotImplementedException();
        }

        public int PlayerToGo()
        {
            if (whiteToGo) return 0;
            return 1;
        }

        public void Restart()
        {
            throw new NotImplementedException();
        }

        public IGameResult Result(int playerId)
        {
            throw new NotImplementedException();
        }

        public void UndoMove()
        {
            throw new NotImplementedException();
        }
    }
}

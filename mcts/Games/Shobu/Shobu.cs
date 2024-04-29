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
        private int[] white_player_white_board = new int[16];
        public int[] white_player_black_board = new int[16];
        private int[] black_player_white_board = new int[16];
        private int[] black_player_black_board = new int[16];

        public Shobu()
        {
            ResetBoard(white_player_white_board);
            ResetBoard(white_player_black_board);
            ResetBoard(black_player_white_board);
            ResetBoard(black_player_black_board);
        }

        private void ResetBoard(int[] board)
        {
            int[] startingPos = [
                1, 1, 1, 1,
                0, 0, 0, 0,
                0, 0, 0, 0,
                -1, -1, -1, -1
            ];
            for (int i = 0; i < 16; i++) 
            {
                board[i] = startingPos[i];
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

        private IEnumerable<int> ShortMoves(int from)
        {
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    int to = from + i + (4 * j);
                    if (ValidTile(to) && to != from) yield return to;
                }
            }
        }

        private IEnumerable<int> Moves(int from)
        {
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    int dir = i + (4 * j);
                    if (ValidTile(from + dir) && from + dir != from) yield return from + dir; // short move
                    if (ValidTile(from + (2 * dir)) && from + (2 * dir) != from) yield return from + (2 * dir); // long move
                }
            }
        }

        private bool ValidTile(int tile) => (tile >= 0 && tile < 16);
        private bool ValidAggressive(int[] board, int from, int to)
        {
            return false;
        }

        private int[] BoardForPlayer(bool whitePlayer, bool whiteBoard)
        {
           if (whitePlayer && whiteBoard) return white_player_white_board;
           if (whitePlayer && !whiteBoard) return white_player_black_board;
           if (!whitePlayer && whiteBoard) return black_player_white_board;
           return black_player_black_board;
        }

        private IEnumerable<Move> MovesForBoard(bool whiteHomeBoard)
        {
            List<Move> passiveOnly = new List<Move>();
            int[] homeBoard = BoardForPlayer(whiteToGo, whiteHomeBoard);
            int[] opponentBoard = BoardForPlayer(!whiteToGo, !whiteHomeBoard);
            int piece = whiteToGo ? 1 : -1;
            foreach (int from in PieceIterator(homeBoard, whiteToGo))
            {
                foreach (int to in ShortMoves(from))
                {
                    if (homeBoard[to] == piece)
                    {
                        Move move = new Move()
                        {
                            WhitePassive = whiteHomeBoard,
                            PassiveFrom = from,
                            PassiveTo = to
                        };
                        passiveOnly.Add(move);
                    }
                }
            }
            foreach (Move passiveMove in passiveOnly)
            {
                int dir = passiveMove.PassiveTo - passiveMove.PassiveFrom;
                foreach (int from in PieceIterator(opponentBoard, whiteToGo))
                {
                    int to = from + dir;
                    if (!ValidTile(to) || opponentBoard[to] == piece) continue;
                    if (opponentBoard[to] == -piece && ValidTile(to + dir) && opponentBoard[to + dir] != 0) continue;
                    Move move = new Move()
                    {
                        WhitePassive = passiveMove.WhitePassive,
                        PassiveFrom = passiveMove.PassiveFrom,
                        PassiveTo = passiveMove.PassiveTo,
                        AggressiveFrom = from
                    };
                    yield return move;
                }
            }
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

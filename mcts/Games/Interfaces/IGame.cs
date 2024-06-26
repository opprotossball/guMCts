﻿using System;
using System.Collections.Generic;

namespace mcts.Games.Interfaces
{
    public interface IGame
    {
        public List<IMove> GetLegalMoves();
        public int PlayerToGo();
        public void UndoMove();
        public void MakeMove(IMove move);
        public bool IsOverForPlayers();
        public GameResult Result(int playerId);
        //position hash
        public int GetHashCode();
        public IGame HistorylessCopy();
        public IMove PseudoRandomMove(Random random);
    }
}

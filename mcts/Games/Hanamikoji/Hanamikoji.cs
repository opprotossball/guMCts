using mcts.Games.Interfaces;


namespace mcts.Games.Hanamikoji
{
    public class Hanamikoji : IGame
    {
        private readonly int[] CardCounts = new int[] { 2, 2, 2, 3, 3, 4, 5 };
        private readonly List<Geishas> cards;
        private int[] geishaScores;
        private bool[] firstPlayerActions;
        private bool[] secondPlayerActions;
        private Geishas? firstPlayerSecret;
        private Geishas? secondPlayerSecret;
        private List<Geishas> firstPlayerHand;
        private List<Geishas> secondPlayerHand;
        private List<Geishas> played;
        private Stack<Geishas> deck;
        private bool firstPlayerToGo;
        private Stack<Move> moveHistory;

        public Hanamikoji() 
        {
            cards = new List<Geishas>();
            for (int i = 0; i < CardCounts.Length; i++)
            {
                for (int j = 0; j < CardCounts[i]; j++)
                {
                    cards.Add((Geishas)i);
                }
            }
            moveHistory = new Stack<Move>();
            PrepareNewRound();
            firstPlayerToGo = true;
        }

        private void PrepareNewRound()
        {
            played = new List<Geishas>();
            deck = new Stack<Geishas>();
            firstPlayerHand = new List<Geishas>();
            secondPlayerHand = new List<Geishas>();
            // init deck
            foreach (Geishas card in cards.OrderBy(x => Random.Shared.Next()).ToList())
            {
                deck.Push(card);
            }
            // deal 6 cards
            for (int i = 0; i < 6; i++)
            {
                firstPlayerHand.Add(deck.Pop());
                secondPlayerHand.Add(deck.Pop());
            }
            // reset actions
            firstPlayerActions = new bool[4];
            secondPlayerActions = new bool[4];
            Array.Fill(firstPlayerActions, true);
            Array.Fill(secondPlayerActions, true);
            // reset secrets
            firstPlayerSecret = null; 
            secondPlayerSecret = null;
            // reset favors
            geishaScores = new int[7];
        }

        private List<Geishas> PlayerHand(bool firstPlayer)
        {
            return firstPlayer ? firstPlayerHand : secondPlayerHand;
        }

        private bool[] PlayerActions(bool firstPlayer)
        {
            return firstPlayer ? firstPlayerActions : secondPlayerActions;
        }

        public List<Move> GetLegalMoves()
        {
            Move lastMove = moveHistory.Peek();
            if (lastMove != null && !lastMove.Choosing)
            {
                if (lastMove.Action == Actions.Gift) return GenerateGiftChoices();
                else if (lastMove.Action == Actions.Competition) return GenerateCompetitionChoices();
            }
            bool[] actions = PlayerActions(firstPlayerToGo);
            if (actions[(int)Actions.Secret]) return GenerateSecretMoves();
            if (actions[(int)Actions.TradeOff]) return GenerateTradeoffMoves();
            if (actions[(int)Actions.Gift]) return GenerateGiftMoves();
            if (actions[(int)Actions.Competition]) return GenerateCompetitionMoves();
            return new List<Move>();
        }

        public List<Move> GenerateGiftMoves()
        {
            List<Move> res = new List<Move>();
            for (int i = 0; i < 3; i++)
            {
                res.Add(new Move() { Action = Actions.Gift, Choosing = true, Choices = new List<int>() { i } });
            }
            return res;
        }

        public List<Move> GenerateSecretMoves()
        {
            List<Move> res = new List<Move>();
            for (int i = 0; i < 3; i++)
            {
                res.Add(new Move() { Action = Actions.Gift, Choosing = true, Choices = new List<int>() { i } });
            }
            return res;
        }

        public List<Move> GenerateCompetitionMoves()
        {
            List<Move> res = new List<Move>();
            for (int i = 0; i < 3; i++)
            {
                res.Add(new Move() { Action = Actions.Gift, Choosing = true, Choices = new List<int>() { i } });
            }
            return res;
        }

        public List<Move> GenerateTradeoffMoves()
        {
            List<Move> res = new List<Move>();
            for (int i = 0; i < 3; i++)
            {
                res.Add(new Move() { Action = Actions.Gift, Choosing = true, Choices = new List<int>() { i } });
            }
            return res;
        }

        public List<Move> GenerateGiftChoices()
        {
            List<Move> res = new List<Move>();
            for (int i = 0; i < 3; i++)
            {
                res.Add(new Move() { Action = Actions.Gift, Choosing = true, Choices = new List<int>() { i } });
            }
            return res;
        }

        public List<Move> GenerateCompetitionChoices()
        {
            List<Move> res = new List<Move>();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4;)
                {
                    if (i == j) continue;
                    res.Add(new Move() { Action = Actions.Competition, Choosing = true, Choices = new List<int>() { i, j } });
                }
            }
            return res;
        }

        public void MakeMove(IMove move)
        {
            throw new NotImplementedException();
        }

        public int PlayerToGo()
        {
            throw new NotImplementedException();
        }

        public void Restart()
        {
            throw new NotImplementedException();
        }

        public GameResult Result(int playerId)
        {
            throw new NotImplementedException();
        }

        public void UndoMove()
        {
            throw new NotImplementedException();
        }

        List<IMove> IGame.GetLegalMoves()
        {
            throw new NotImplementedException();
        }

        public IGame HistorylessCopy()
        {
            throw new NotImplementedException();
        }
    }
}

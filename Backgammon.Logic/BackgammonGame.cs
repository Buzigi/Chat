using Chat.Contracts;
using Contracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Backgammon.Logic
{
    public class BackgammonGame : INotifyPropertyChanged
    {
        #region Properties and Private Fields


        const string MY_COLOR = "White";

        const string His_COLOR = "Black";

        public ObservableCollection<string>[] Board { get; set; }

        public ObservableCollection<string>[] EndStack { get; set; }

        public ObservableCollection<string>[] Jail { get; set; }

        public ObservableCollection<int> Dice { get; set; }

        public int[] Score { get; set; }


        //List of moves made by the player
        public List<Move> Moves { get; set; }

        //true = Player can start moving pieces to end stack
        public bool isEndPhase { get; set; }

        public ObservableCollection<Move> PossibleMoves { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Properties and Private Fields 

        #region C'tor

        public BackgammonGame()
        {
            Board = new ObservableCollection<string>[24];
            EndStack = new ObservableCollection<string>[2];
            Jail = new ObservableCollection<string>[2];
            Score = new int[] { 0, 0 };
            Dice = new ObservableCollection<int> { 1, 1 };
            PossibleMoves = new ObservableCollection<Move>();
            Moves = new List<Move>();
            isEndPhase = false;
            SetNewGameBoard();
        }


        #endregion C'tor


        #region Public Methods

        public void RollDice(Random rand)
        {
            Dice = new ObservableCollection<int>();
            Dice.Add(rand.Next(1, 7));
            Dice.Add(rand.Next(1, 7));
            if (Dice[0] == Dice[1])
            {
                Dice.Add(Dice[1]);
                Dice.Add(Dice[1]);
            }
            Moves.Clear();
            GetListOfPossibleMoves();
            PropertyChanged(null, new PropertyChangedEventArgs("Dice"));
        }

        //When a piece is selected for move, return the possible stacks it can move to
        public List<int> GetPossibleMoves(int stackNum)
        {
            return PossibleMoves.Where(m => m.FromStack == stackNum).Select(m => m.ToStack).ToList();

        }

        //Logic for moving pieces across the board
        public void MovePiece(int fromstack, int toStack, int player)
        {
            string color;
            string hisColor;
            if (player == 0)
            {
                color = MY_COLOR;
                hisColor = His_COLOR;
            }
            else
            {
                color = His_COLOR;
                hisColor = MY_COLOR;
                if (fromstack != -1)
                {
                    fromstack = 23 - fromstack;
                }
                toStack = 23 - toStack;
            }
            if (player == 0)
            {
                Move newMove = new Move(fromstack, toStack, RemoveDice(fromstack, toStack));
                Moves.Add(newMove);
            }

            //Piece if from jail
            if (fromstack == -1)
            {
                Jail[player].Remove(color);
            }

            //Piece is on board
            else
            {
                Board[fromstack].Remove(color);

            }

            //Piece goes to end stack
            if (toStack == -2)
            {
                EndStack[player].Add(color);
                return;
            }

            //Piece goes to board
            else
            {
                Board[toStack].Add(color);
            }

            //Another player's piece is in the toStack
            if (Board[toStack].Contains(hisColor))
            {
                Board[toStack].Remove(hisColor);
                Jail[1 - player].Add(hisColor);
            }

            //There are other moves in the player's turn
            if (Dice.Count != 0)
            {
                GetListOfPossibleMoves();
            }

        }

        public void MovePieces(List<Move> moves)
        {
            Dice.Clear();
            foreach (Move move in moves)
            {
                Dice.Add(move.Dice);
            }
            foreach (Move move in moves)
            {
                MovePiece(move.FromStack, move.ToStack, 1);
            }
        }


        #endregion Public Methods


        #region Private Methods

        private void SetNewGameBoard()
        {
            for (int i = 0; i < 24; i++)
            {
                Board[i] = new ObservableCollection<string>();
            }

            AddStackToBoard(23, MY_COLOR, 2);
            AddStackToBoard(12, MY_COLOR, 5);
            AddStackToBoard(7, MY_COLOR, 3);
            AddStackToBoard(5, MY_COLOR, 5);

            AddStackToBoard(0, His_COLOR, 2);
            AddStackToBoard(11, His_COLOR, 5);
            AddStackToBoard(16, His_COLOR, 3);
            AddStackToBoard(18, His_COLOR, 5);
            PropertyChanged?.Invoke(null, new PropertyChangedEventArgs("Board"));

            Jail[0] = new ObservableCollection<string>(); // { "White", "White" };
            Jail[1] = new ObservableCollection<string>(); //{ "Black", "Black", "Black" };
            EndStack[0] = new ObservableCollection<string>(); //{ "White", "White", "White" };
            EndStack[1] = new ObservableCollection<string>(); //{ "Black", "Black", "Black" };
        }

        private int RemoveDice(int fromstack, int toStack)
        {
            int dice = PossibleMoves.FirstOrDefault(m => m.FromStack == fromstack && m.ToStack == toStack).Dice;
            Dice.Remove(dice);
            PropertyChanged?.Invoke(null, new PropertyChangedEventArgs("Dice"));
            if (Dice.Count != 0)
            {
                GetListOfPossibleMoves();
            }
            return dice;
        }

        private void AddStackToBoard(int index, string color, int numOfPieces)
        {
            for (int i = 0; i < numOfPieces; i++)
            {
                Board[index].Add(color);
            }
        }

        //Returns list of stacks a piece can move to
        private List<int> GetOpenStacks()
        {
            List<int> openStacks = new List<int>();
            for (int i = 0; i < 24; i++)
            {
                //no home in the ith stack
                if (Board[i].Count <= 1)
                {
                    openStacks.Add(i);
                }

                //Player's pieces in the stack
                else if (Board[i][0] == MY_COLOR)
                {
                    openStacks.Add(i);
                }
            }
            return openStacks;
        }

        private void GetListOfPossibleMoves()
        {
            PossibleMoves.Clear();
            List<int> openStacks = GetOpenStacks();

            int diceRoll;

            int numOfDice;

            if (Dice.Count == 1)
            {
                numOfDice = 1;
            }
            else if (Dice[0] == Dice[1])
            {
                numOfDice = 1;
            }

            else
            {
                numOfDice = 2;
            }

            //Check moves for each dice
            for (int i = 0; i < numOfDice; i++)
            {
                diceRoll = Dice[i];
                //If Player has a piece in jail, the move is to get it out
                if (Jail[0].Count != 0)
                {
                    //Dice number corresponds to open stack
                    if (openStacks.Contains(23 - diceRoll + 1))
                    {
                        PossibleMoves.Add(new Move(-1, 23 - diceRoll + 1, diceRoll));
                    }
                }

                //No Player's pieces in jail
                else
                {
                    for (int j = 0; j < 24; j++)
                    {
                        //Stack is not empty and contains Player's pieces
                        if (Board[j].Count != 0 && Board[j][0] == MY_COLOR)
                        {
                            if (openStacks.Contains(j - diceRoll))
                            {
                                PossibleMoves.Add(new Move(j, j - diceRoll, diceRoll));
                            }

                            //If end stack is available
                            else if (isEndPhase && j - diceRoll < 0)
                            {
                                PossibleMoves.Add(new Move(j, -2, diceRoll));
                            }
                        }
                    }
                }
            }
        }


        #endregion Private Methods

    }

    //A move is a piece moving from stack to stack

}

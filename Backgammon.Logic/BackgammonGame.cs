using Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backgammon.Logic
{
    public class BackgammonGame : INotifyPropertyChanged
    {
        #region Properties and Private Fields

        Random rand;

        const string MY_COLOR = "White";

        public List<string>[] Board { get; set; }

        public List<string>[] EndStack { get; set; }

        public List<string>[] Jail { get; set; }

        public List<int> Dice { get; set; }

        public int[] Score { get; set; }

        //true = Player can start moving pieces to end stack
        public bool isEndPhase { get; set; }
        
        public List<Move> PossibleMoves { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Properties and Private Fields 

        #region C'tor

        public BackgammonGame()
        {
            Board = new List<string>[24];
            EndStack = new List<string>[2];
            Jail = new List<string>[2];
            Score = new int[] { 0, 0 };
            Dice = new List<int> { 1, 1};
            PossibleMoves = new List<Move>();
            rand = new Random();
            isEndPhase = true;
            SetNewGameBoard();
        }


        #endregion C'tor


        #region Public Methods

        public void RollDice()
        {
            Dice = new List<int>();
            Dice.Add(rand.Next(1, 7));
            Dice.Add(rand.Next(1, 7));
            if (Dice[0] == Dice[1])
            {
                Dice.Add(Dice[1]);
                Dice.Add(Dice[1]);
            }
            GetListOfPossibleMoves();
            PropertyChanged(null, new PropertyChangedEventArgs("Dice"));
        }

        //When a piece is selected for move, return the possible stacks it can move to
        public List<int> GetPossibleMoves(int stackNum)
        {
           return PossibleMoves.Where(m => m.FromStack == stackNum).Select(m=>m.ToStack).ToList();

        }
        

        #endregion Public Methods


        #region Private Methods

        private void SetNewGameBoard()
        {
            for (int i = 0; i < 24; i++)
            {
                Board[i] = new List<string>();
            }

            AddStackToBoard(23, MY_COLOR, 2);
            AddStackToBoard(12, MY_COLOR, 5);
            AddStackToBoard(7, MY_COLOR, 3);
            AddStackToBoard(5, MY_COLOR, 5);

            AddStackToBoard(2, MY_COLOR, 1);

            string color = "Black";

            AddStackToBoard(0, color, 2);
            AddStackToBoard(11, color, 5);
            AddStackToBoard(16, color, 3);
            AddStackToBoard(18, color, 5);
            PropertyChanged?.Invoke(null, new PropertyChangedEventArgs("Board"));

            Jail[0] = new List<string>(); //{ "White", "White" };
            Jail[1] = new List<string>(); //{ "Black", "Black", "Black" };
            EndStack[0] = new List<string>(); //{ "White", "White", "White" };
            EndStack[1] = new List<string>(); //{ "Black", "Black", "Black" };
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
                        PossibleMoves.Add(new Move(-1, 23 - diceRoll + 1));
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
                                PossibleMoves.Add(new Move(j, j - diceRoll));
                            }

                            //If end stack is available
                            else if (isEndPhase && j - diceRoll < 0)
                            {
                                PossibleMoves.Add(new Move(j, -2));
                            }
                        }
                    }
                }
            }
        }

        #endregion Private Methods

    }

    //A move is a piece moving from stack to stack
    public class Move
    {
        //0-23: Board stacks
        //-1: Jail
        //-2: End stack

        public int FromStack { get; set; }

        public int ToStack { get; set; }

        public Move(int from, int to)
        {
            FromStack = from;
            ToStack = to;
        }
    }
}

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

        public List<string>[] Board { get; set; }

        public List<string> EndStack { get; set; }

        public List<string> Jail { get; set; }

        public int[] Dice { get; set; }

        public int[] Score { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Properties and Private Fields 

        #region C'tor

        public BackgammonGame()
        {
            Board = new List<string>[24];
            EndStack = new List<string>();
            Jail = new List<string>();
            Score = new int[] { 0, 0 };
            Dice = new int[] { 0, 0 };
            rand = new Random();
            SetNewGameBoard();
        }


        #endregion C'tor


        #region Public Methods

        public void RollDice()
        {
            Dice[0] = rand.Next(0, 6);
            Dice[1] = rand.Next(0, 6);
            PropertyChanged(null, new PropertyChangedEventArgs("Dice"));
        }


        #endregion Public Methods


        #region Private Methods

        private void SetNewGameBoard()
        {
            for (int i = 0; i < 24; i++)
            {
                Board[i] = new List<string>();
            }
            string color = "White";

            AddToBoard(23, color, 2);
            AddToBoard(12, color, 5);
            AddToBoard(7, color, 3);
            AddToBoard(5, color, 5);

            color = "Black";

            AddToBoard(0, color, 2);
            AddToBoard(11, color, 5);
            AddToBoard(16, color, 3);
            AddToBoard(18, color, 5);
            PropertyChanged?.Invoke(null, new PropertyChangedEventArgs("Board"));
        }

        private void AddToBoard(int index, string color, int numOfPieces)
        {
            for (int i = 0; i < numOfPieces; i++)
            {
                Board[index].Add(color);
            }
        }


        #endregion Private Methods

        #region Public Methods

        public void GetListOfPossibleMoves()
        {
            throw new NotImplementedException();
        }


        #endregion Public Methods
    }
}

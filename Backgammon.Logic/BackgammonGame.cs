using Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backgammon.Logic
{
    public class BackgammonGame: INotifyPropertyChanged
    {
        #region Properties and Private Fields

        private Game _game;

        public Game Game { get
            {
                return _game;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Properties and Private Fields 

        #region C'tor

        public BackgammonGame()
        {
            _game = new Game()
            {
                Board = new int[2,24],
                EndStack = new int[2],
                Jail = new int[2],
                Score = new int[]{ 0, 0 }
            };
            SetNewGameBoard();
        }


        #endregion C'tor


        #region Public Methods

        public Game UpdateGameStatus()
        {
            return _game;
        }

        public void SetGameStatus(Game game)
        {
            _game = game;
        }

        #endregion Public Methods


        #region Private Methods

        private void SetNewGameBoard()
        {
            for (int i = 0; i < 2; i++)
            {
                _game.Board[i, 23] = 2;
                _game.Board[i, 12] = 5;
                _game.Board[i, 7] = 3;
                _game.Board[i, 5] = 5;
            }
            _game.Jail[0] = 0;
            _game.Jail[1] = 0;
            _game.EndStack[0] = 0;
            _game.EndStack[1] = 0;
            PropertyChanged?.Invoke(null, new PropertyChangedEventArgs("Game"));
        }


        #endregion Private Methods
    }
}

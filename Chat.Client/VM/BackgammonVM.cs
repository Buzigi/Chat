using Backgammon.Logic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Client.VM
{
    public class BackgammonVM : INotifyPropertyChanged
    {
        #region Properties and Fields

        BackgammonGame game;

        public List<string> Color { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public string PlayerA { get; set; }

        public string PlayerB { get; set; }


        #endregion Properties and Fields


        #region C'tor

        public BackgammonVM(string playerA, string playerB)
        {
            PlayerA = playerA;
            PlayerB = playerB;
            PropertyChanged?.Invoke(null, new PropertyChangedEventArgs("PlayerA"));
            PropertyChanged?.Invoke(null, new PropertyChangedEventArgs("PlayerB"));

            game = new BackgammonGame();
            //UpdateBoard();
        }


        #endregion C'tor

        #region Private Methods
        private void UpdateBoard()
        {
            throw new NotImplementedException();
        }

        #endregion Private Methods
    }
}

using Backgammon.Logic;
using Chat.Logic;
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

        const int NUM_OF_STACKS = 24;

        private BackgammonGame _game;

        public List<string>[] Stacks { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public string PlayerA { get; set; }

        public string PlayerB { get; set; }

        public Guid? Session { get; set; }


        #endregion Properties and Fields
        

        #region C'tor

        public BackgammonVM(string playerA, string playerB)
        {
            PlayerA = playerA;
            PlayerB = playerB;
            PropertyChanged?.Invoke(null, new PropertyChangedEventArgs("PlayerA"));
            PropertyChanged?.Invoke(null, new PropertyChangedEventArgs("PlayerB"));
            Stacks = new List<string>[NUM_OF_STACKS];
            for (int i = 0; i < NUM_OF_STACKS; i++)
            {
                Stacks[i] = new List<string>();
            }
            _game = new BackgammonGame();
        }


        #endregion C'tor

        #region Public Methods

        public void StartChat()
        {
            Session = ChatClient.AddSession(PlayerA, PlayerB);
        }

        public void AddColor(string col, int index)
        {
            Stacks[index].Add(col);

            PropertyChanged?.Invoke(null, new PropertyChangedEventArgs("Colors"));
        }

        #endregion Public Methods


        #region Private Methods


        #endregion Private Methods
    }
}

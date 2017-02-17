using Backgammon.Logic;
using Chat.Logic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Chat.Client.VM
{
    public class BackgammonVM : INotifyPropertyChanged
    {
        #region Properties and Fields

        public BitmapImage[] DiceImages { get; set; }
        
        public BackgammonGame Game { get; set; }

        public string PlayerA { get; set; }

        public string PlayerB { get; set; }

        public Guid? Session { get; set; }

        public BitmapImage[] DiceImage { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Properties and Fields


        #region C'tor

        public BackgammonVM(string playerA, string playerB)
        {
            PlayerA = playerA;
            PlayerB = playerB;
            PropertyChanged?.Invoke(null, new PropertyChangedEventArgs("PlayerA"));
            PropertyChanged?.Invoke(null, new PropertyChangedEventArgs("PlayerB"));
            InitDiceImageList();
            
            Game = new BackgammonGame();
            Game.PropertyChanged += Game_PropertyChanged;
        }


        #endregion C'tor

        #region Public Methods

        public void StartChat()
        {
            Session = ChatClient.AddSession(PlayerA, PlayerB);
        }



        #endregion Public Methods


        #region Private Methods

        private void InitDiceImageList()
        {
            DiceImage = new BitmapImage[2];
            DiceImages = new BitmapImage[6];
            DiceImages[0] = new BitmapImage(new Uri("../../Images/1.PNG", UriKind.Relative));
            DiceImages[1] = new BitmapImage(new Uri("../../Images/2.PNG", UriKind.Relative));
            DiceImages[2] = new BitmapImage(new Uri("../../Images/3.PNG", UriKind.Relative));
            DiceImages[3] = new BitmapImage(new Uri("../../Images/4.PNG", UriKind.Relative));
            DiceImages[4] = new BitmapImage(new Uri("../../Images/5.PNG", UriKind.Relative));
            DiceImages[5] = new BitmapImage(new Uri("../../Images/6.PNG", UriKind.Relative));
            DiceImage[0] = DiceImages[0];
            DiceImage[1] = DiceImages[0];
            PropertyChanged?.Invoke(null, new PropertyChangedEventArgs("DiceImage"));
        }
        

        private void Game_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Board")
            {
                PropertyChanged?.Invoke(null, new PropertyChangedEventArgs("Game)"));
            }
            else if (e.PropertyName == "Dice")
            {
                DiceImage[0] = DiceImages[Game.Dice[0]];
                DiceImage[1] = DiceImages[Game.Dice[1]];
                PropertyChanged?.Invoke(null, new PropertyChangedEventArgs("DiceImage"));
            }
        }

        #endregion Private Methods
    }
}

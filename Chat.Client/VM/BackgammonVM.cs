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

        Random rand;

        BitmapImage[] _diceImagesList { get; set; }

        public BackgammonGame Game { get; set; }

        public string PlayerA { get; set; }

        public string PlayerB { get; set; }

        public Guid? Session { get; set; }

        public List<BitmapImage> DiceImage { get; set; }

        public bool IsWaiting { get; set; }

        public bool IsWaitingForMove { get; set; }

        public int PieceToMove { get; set; }

        public List<int> MovesPerPiece { get; set; }


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
            IsWaitingForMove = false;
            Game = new BackgammonGame();
            Game.PropertyChanged += Game_PropertyChanged;
            rand = new Random();
        }


        #endregion C'tor

        #region Public Methods

        public void StartChat()
        {
            Session = ChatClient.AddSession(PlayerA, PlayerB);
        }

        public void GetPossibleMovesPerPiece(int pieceStack)
        {
            MovesPerPiece = Game.GetPossibleMoves(pieceStack);
        }

        #endregion Public Methods


        #region Private Methods

        private void InitDiceImageList()
        {
            _diceImagesList = new BitmapImage[6];
            _diceImagesList[0] = new BitmapImage(new Uri("../../Images/1.PNG", UriKind.Relative));
            _diceImagesList[1] = new BitmapImage(new Uri("../../Images/2.PNG", UriKind.Relative));
            _diceImagesList[2] = new BitmapImage(new Uri("../../Images/3.PNG", UriKind.Relative));
            _diceImagesList[3] = new BitmapImage(new Uri("../../Images/4.PNG", UriKind.Relative));
            _diceImagesList[4] = new BitmapImage(new Uri("../../Images/5.PNG", UriKind.Relative));
            _diceImagesList[5] = new BitmapImage(new Uri("../../Images/6.PNG", UriKind.Relative));
            DiceImage = new List<BitmapImage>();
            DiceImage.Add(_diceImagesList[0]);
            DiceImage.Add(_diceImagesList[0]);
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
                DiceImage = new List<BitmapImage>();
                foreach (int dice in Game.Dice)
                {
                    DiceImage.Add(_diceImagesList[dice - 1]);
                }
                PropertyChanged?.Invoke(null, new PropertyChangedEventArgs("DiceImage"));
            }
        }

        internal void AnimateDiceRoll()
        {
            DiceImage = new List<BitmapImage>();
            DiceImage.Add(_diceImagesList[rand.Next(0,6)]);
            DiceImage.Add(_diceImagesList[rand.Next(0, 6)]);
            PropertyChanged?.Invoke(null, new PropertyChangedEventArgs("DiceImage"));
        }

        #endregion Private Methods
    }
}

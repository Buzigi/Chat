using Backgammon.Logic;
using Chat.Contracts;
using Chat.Logic;
using Chat.UI;
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


        BitmapImage[] _diceImagesList { get; set; }

        public BackgammonGame Game { get; set; }

        public string PlayerA { get; set; }

        public string PlayerB { get; set; }

        public Guid? Session { get; set; }

        public List<BitmapImage> DiceImage { get; set; }

        private bool _isWaiting;
        public bool IsWaiting
        {
            get { return _isWaiting; }
            set
            {
                _isWaiting = value;
                PropertyChanged?.Invoke(null, new PropertyChangedEventArgs("IsWaiting"));
            }
        }

        public bool IsWaitingForMove { get; set; }

        public double ControlVisibility { get; set; }

        int _pieceToMove;
        public int PieceToMove
        {
            get
            {
                return _pieceToMove;
            }
            set
            {
                _pieceToMove = value;
                GetPossibleMovesPerPiece();
            }
        }

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
            ChatResult.MoveRecievedEvent += ChatResult_MoveRecievedEvent;
        }



        #endregion C'tor

        #region Public Methods

        public void StartChat()
        {
            Session = ChatClient.AddSession(PlayerA, PlayerB);
        }

        public void GetPossibleMovesPerPiece()
        {
            MovesPerPiece = Game.GetPossibleMoves(PieceToMove);
        }

        internal void AnimateDiceRoll(int d1, int d2)
        {
            DiceImage = new List<BitmapImage>();
            DiceImage.Add(_diceImagesList[d1 - 1]);
            DiceImage.Add(_diceImagesList[d2 - 1]);
            PropertyChanged?.Invoke(null, new PropertyChangedEventArgs("DiceImage"));
        }

        internal void SendMovesToOtherPlayer()
        {
            Game.Dice.Clear();
            ChatClient.SendMove((Guid)Session, Game.Moves);
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

        #endregion Private Methods

        #region Event Handling

        private void Game_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Board")
            {
                PropertyChanged?.Invoke(null, new PropertyChangedEventArgs("Game"));
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
            else if (e.PropertyName == "0")
            {
                PropertyChanged?.Invoke(null, new PropertyChangedEventArgs("0"));
            }
            else if (e.PropertyName == "1")
            {
                PropertyChanged?.Invoke(null, new PropertyChangedEventArgs("1"));
            }
        }

        private void ChatResult_MoveRecievedEvent(object sender, EventArgs e)
        {
            string contact = ((MoveEventArgs)e).Contact;
            if (contact == PlayerB)
            {
                List<Move> moves = ((MoveEventArgs)e).Moves;
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    Game.MovePieces(moves);
                    IsWaiting = false;
                });
            }


        }

        #endregion Event Handling
    }
}

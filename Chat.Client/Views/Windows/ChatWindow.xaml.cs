using Chat.Client.VM;
using Chat.UI.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Chat.UI.Views.Screens
{
    /// <summary>
    /// Interaction logic for ChatWindow.xaml
    /// </summary>
    public partial class ChatWindow : Window
    {
        MainVM _mainVM;


        BackgammonVM gameVM;

        string _contact;

        public bool IsGame { get; set; }


        public event EventHandler WindowClosedEvent;

        public ChatWindow(string contact, bool isGame = false, bool isMyTurn = false, Guid? session = null)
        {
            InitializeComponent();
            _mainVM = MainVM.Instance;
            _contact = contact;
            IsGame = isGame;

            if (IsGame)
            {
                this.Title = $"{_mainVM.UserName} - Game with {_contact}";
                this.Width = 1200;
                this.Height = 700;
                gameVM = new BackgammonVM(_mainVM.UserName, contact);
                gameVM.Session = session;
                gameVM.IsWaiting = !isMyTurn;
                chatControl_cc.Content = new BackgammonScreen(gameVM);
                gameVM.PropertyChanged += GameVM_GameEnded;
            }
            else
            {
                this.Title = $"{_mainVM.UserName} - Chat with {_contact}";
                chatControl_cc.Content = new ChatScreen(_mainVM.UserName, contact);
            }
        }

        private void GameVM_GameEnded(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "0")
            {
                EndGame(0);
            }
            else if (e.PropertyName == "1")
            {
                EndGame(1);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Guid? session;
            if (gameVM != null)
            {
                session = (Guid)gameVM.Session;
            }
            else
            {
                session = null;
            }
            WindowClosedEvent?.Invoke(this, new ChatEventArgs()
            {
                contact = _contact,
                Session = session
            });
        }

        internal void CloseChat()
        {
            this.Close();
        }


        private void EndGame(int player)
        {
            string message;
            if (player == 0)
            {
                message = "You Win!";
                gameVM.IsWaiting = false;
                gameVM.SendMovesToOtherPlayer();
            }
            else
            {
                message = "You Lose!";
                gameVM.IsWaiting = true;
            }
            MessageBox.Show(message);
            chatControl_cc.Content = new BackgammonScreen(gameVM);
        }
    }

    public class ChatEventArgs : EventArgs
    {
        public string contact { get; set; }

        public Guid? Session { get; set; }

    }
}

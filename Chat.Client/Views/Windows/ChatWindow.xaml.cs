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

        string _contact;

        public bool IsGame { get; set; }


        public event EventHandler WindowClosedEvent;

        public ChatWindow(string contact, bool isGame = false)
        {
            InitializeComponent();
            _mainVM = MainVM.Instance;
            _contact = contact;
            IsGame = isGame;
            
            if (IsGame)
            {
                this.Title = $"{_mainVM.UserName} - Game with {_contact}";
                this.Width = 1000;
                this.Height = 700;
                BackgammonVM gameVM = new BackgammonVM(_mainVM.UserName, contact);
                chatControl_cc.Content = new BackgammonScreen(gameVM);
            }
            else
            {
                this.Title = $"{_mainVM.UserName} - Chat with {_contact}";
                chatControl_cc.Content = new ChatScreen(_mainVM.UserName, contact);
            }
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            WindowClosedEvent?.Invoke(this, new ChatEventArgs() { contact = _contact });
        }

        internal void CloseChat()
        {
            this.Close();
        }
    }

    public class ChatEventArgs : EventArgs
    {
        public string contact { get; set; }
    }
}

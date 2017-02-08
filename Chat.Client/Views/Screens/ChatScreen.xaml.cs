using Chat.Logic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Chat.UI.UserControls
{
    /// <summary>
    /// Interaction logic for ChatScreen.xaml
    /// </summary>
    public partial class ChatScreen : UserControl
    {
        ChatVM _chatVM;

        public ChatScreen(string sender, string reciever)
        {
            InitializeComponent();
            _chatVM = new ChatVM(sender,reciever);
            Task.Run(new Action(() => _chatVM.StartChat()));
            _chatVM.PropertyChanged += _chatVM_PropertyChanged;
        }

        private void _chatVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(new System.Action(() =>
            {
                messageList_lb.ItemsSource = _chatVM.Messages;
                messageList_lb.SelectedIndex = messageList_lb.Items.Count - 1;
                messageList_lb.ScrollIntoView(messageList_lb.SelectedItem);
            }));
        }

        private void send_b_Click(object sender, RoutedEventArgs e)
        {
            string message = message_tb.Text;
            if (message!="")
            {
                Task.Run(new Action(() => ChatClient.SendMessage((Guid)_chatVM.Session, message)));
                
            }
            message_tb.Text = "";
        }

        private void send_Enter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                send_b_Click(null, null);
            }
        }
    }
}

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

namespace Chat.UI.UserControls
{
    /// <summary>
    /// Interaction logic for ChatWindow.xaml
    /// </summary>
    public partial class ChatWindow : Window
    {
        MainVM _mainVM;

        string _contact;

        public event EventHandler WindowClosedEvent;

        public ChatWindow(string contact)
        {
            InitializeComponent();
            _mainVM = MainVM.Instance;
            _contact = contact;
            
            this.Title = $"{_mainVM.UserName} - Chat with {_contact}";
            chatControl_cc.Content = new ChatScreen(_mainVM.UserName,contact);
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

    public class ChatEventArgs: EventArgs
    {
        public string contact { get; set; }
    }
}

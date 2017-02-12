using Chat.UI.UserControls;
using Chat.UI.VM;
using Helpers;
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

namespace Chat.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainVM _mainVM;

        //Fires event when the connection menu item is clicked
        public static event EventHandler ConnectionEvent;

        //Fires event when the Send Message menu item is clicked
        public static event EventHandler OpenChatEvent;

        //Fires event when the Play Backgammon menu item is clicked
        public static event EventHandler PlayEvent;

        public MainWindow()
        {
            InitializeComponent();
            _mainVM = MainVM.Instance;
            this.DataContext = _mainVM;

            //Subscribe to change of screen to set menu item function
            _mainVM.DisplayContactEvent += ChangeMenuToDisconnect;
            _mainVM.DisplayWelcomeEvent += ChangeMenuToConnect;
            Report.log(DeviceToReport.Client_MainWindow, LogLevel.Information, $"Client window");

        }

        //Welcome screen is displayed
        private void ChangeMenuToConnect(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new System.Action(() =>
            {
                connection_mi.Header = "_Connect";
                connection_mi.Click += Connect_mi_Click;
                connection_mi.Click -= Disconnect_mi_Click;
                actions_mi.IsEnabled = false;
            }));
        }

        //Contact screen is displayed
        private void ChangeMenuToDisconnect(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new System.Action(() =>
            {
                connection_mi.Header = "_Disconnect";
                connection_mi.Click += Disconnect_mi_Click;
                connection_mi.Click -= Connect_mi_Click;
                actions_mi.IsEnabled = true;
            }));
        }


        private void Connect_mi_Click(object sender, RoutedEventArgs e)
        {
            ConnectionEvent?.Invoke(null, new ConnectionEventArgs(true));
        }

        private void Disconnect_mi_Click(object sender, RoutedEventArgs e)
        {
            ConnectionEvent?.Invoke(null, new ConnectionEventArgs(false));
        }

        private void sendMessage_mi_Click(object sender, RoutedEventArgs e)
        {
            OpenChatEvent?.Invoke(null, null);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Disconnect_mi_Click(null, null);
        }

        private void play_mi_Click(object sender, RoutedEventArgs e)
        {
            PlayEvent?.Invoke(null, null);
        }
    }

    public class ConnectionEventArgs: EventArgs
    {
        public bool Connect { get; set; }

        public ConnectionEventArgs(bool connect)
        {
            Connect = connect;
        }
    }

}

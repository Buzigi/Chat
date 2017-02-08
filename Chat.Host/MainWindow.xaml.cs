using Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
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

namespace Host
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ServiceHost host;
        string _logs;
        
        public MainWindow()
        {
            InitializeComponent();
            stopService_b.IsEnabled = false;
            _logs = "ServiceLog" + DateTime.Now.ToLongDateString();
            ChatService.ReportEvent += ChatService_ReportEvent;

        }

        private void ChatService_ReportEvent(object sender, EventArgs e)
        {
            string message = ((ChatServiceEventArgs)e).Message;
            log_tb.Text = message;
            log_tb.Foreground = Brushes.Black;
            log_tb.FontSize = 15;
            File.AppendAllText(_logs, message + Environment.NewLine);
        }

        private void startService_b_Click(object sender, RoutedEventArgs e)
        {
            host = new ServiceHost(typeof(ChatService));
            try
            {
                host.Open();
                startService_b.IsEnabled = false;
                stopService_b.IsEnabled = true;
                log_tb.FontSize = 30;
                log_tb.Text = "Connected";
                log_tb.Foreground = Brushes.Green;
                this.Title = "Host - Connected";

            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }


        private void stopService_b_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                host.Close();
                log_tb.Foreground = Brushes.Black;
                log_tb.FontSize = 30;
                log_tb.Text = "Not Connected";
                startService_b.IsEnabled = true;
                stopService_b.IsEnabled = false;
                this.Title = "Host - Disconnected";
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }
        
        private void HandleException(Exception ex)
        {
            try
            {
                host.Abort();
            }
            catch (Exception)
            {                
            }
            log_tb.Text = ex.Message;
            log_tb.FontSize = 11;
            log_tb.Foreground = Brushes.Red;
            startService_b.IsEnabled = true;
            stopService_b.IsEnabled = false;
            this.Title = "Host - Disconnected";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            startService_b_Click(null, null);
        }
    }
}

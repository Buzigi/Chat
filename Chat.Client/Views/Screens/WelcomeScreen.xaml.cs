using Chat.Logic;
using Chat.UI.VM;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace Chat.UI.UserControls
{
    /// <summary>
    /// Interaction logic for WelcomeScreen.xaml
    /// </summary>
    public partial class WelcomeScreen : UserControl
    {
        #region Properties and Fields

        MainVM _mainVM = MainVM.Instance;

        #endregion Properties and Fields

        #region C'tor

        public WelcomeScreen()
        {
            InitializeComponent();
            this.DataContext = _mainVM;
            _mainVM.IsWaiting = false;
            MainWindow.ConnectionEvent += Menu_ConnectionEvent;
        }
        

        #endregion C'tor

        #region Public Methods

        private void signIn_b_Click(object sender, RoutedEventArgs e)
        {
            _mainVM.Error = "";
            string userName = userName_tb.Text;
            string password = password_tb.Text;
            _mainVM.IsWaiting = true;

            Task.Run(new Action(()=>SignIn(userName,password)));

        }

        private void register_b_Click(object sender, RoutedEventArgs e)
        {
            
            string userName = userName_tb.Text;
            string password = password_tb.Text;
            if (userName != "" && password != "")
            {
                _mainVM.Error = "";
                _mainVM.IsWaiting = true;

                Task.Run(new Action(() => Register(userName, password))); 
            }
            else
            {
                ReportError(LogLevel.Warning, "The Username and Password cannot be blank!");
            }
        }


        #endregion UI Buttons

        #region Private Methods
        
        private void Menu_ConnectionEvent(object sender, EventArgs e)
        {
            bool isConnect = ((ConnectionEventArgs)e).Connect;
            if (isConnect)
            {
                signIn_b_Click(null, null);
            }
        }

        private void SignIn(string userName, string password)
        {
            try
            {
                string error = ChatClient.Login(userName, password);
                if (error == "")
                {
                    Dispatcher.BeginInvoke(new System.Action(() =>
                    {
                        password_tb.Text = "";

                    }));
                    
                    _mainVM.UserControlToDisplay = ScreensEnum.Contact;
                }
                else
                {
                    ReportError(LogLevel.Warning,error);
                    _mainVM.IsWaiting = false;

                }
            }
            catch (Exception e)
            {
                ReportError(LogLevel.Exception, e.Message);
                _mainVM.IsWaiting = false;
            }
        }


        private void Register(string userName, string password)
        {
            try
            {
                if (ChatClient.Register(userName, password))
                {
                    Dispatcher.BeginInvoke(new System.Action(() =>
                    {
                        password_tb.Text = "";

                    }));
                    _mainVM.UserControlToDisplay = ScreensEnum.Contact;
                }
                else
                {
                    ReportError(LogLevel.Warning, $"UserName {userName} is taken, please choose another");
                    _mainVM.IsWaiting = false;

                }
            }
            catch (Exception e)
            {
                ReportError(LogLevel.Exception, e.Message);
                _mainVM.IsWaiting = false;
            }
        }

        private void ReportError(LogLevel level, string message)
        {
            _mainVM.Error = message;
            Report.log(DeviceToReport.Client_WelcomeWindow, level, message);
        }

        #endregion Private Methods

        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                signIn_b_Click(null, null);
            }
        }
    }

}

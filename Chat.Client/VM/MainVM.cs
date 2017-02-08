using Chat.Logic;
using Chat.UI.UserControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.UI.VM
{
    public class MainVM : INotifyPropertyChanged
    {
        #region Properties and Private Fields

        //Current logged on user
        public string UserName { get; set; }

        //Waiting screen setter
        private bool _isWaiting;
        public bool IsWaiting
        {
            get { return _isWaiting; }
            set
            {
                _isWaiting = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsWaiting"));
            }
        }

        //Current displayed screen setter
        private ScreensEnum _controlToDisplay;
        public ScreensEnum UserControlToDisplay
        {
            get
            {
                return _controlToDisplay;
            }
            set
            {
                _controlToDisplay = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("UserControlToDisplay"));
                FireChanegScreenEvent();
            }
        }

        //Error message to display
        private string _error;
        public string Error
        {
            get
            {
                return _error;
            }
            set
            {
                _error = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Error"));
            }
        }

        //Contact lists
        private List<string> _connected;
        public List<string> ConnectedContacts
        {
            get
            {
                return _connected;
            }
            set
            {
                _connected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ConnectedContacts"));
            }
        }

        private List<string> _disconnected;
        public List<string> DisconnectedContacts
        {
            get
            {
                return _disconnected;
            }
            set
            {
                _disconnected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DisconnectedContacts"));
            }
        }


        #endregion Properties and Private Fields


        #region C'tor
        private MainVM()
        {
            UserControlToDisplay = ScreensEnum.Welcome;
            ChatResult.LoggedInEvent += ChatResult_loggedInEvent;
            ChatClient.ConnectionLostEvent += ChatClient_ConnectionLostEvent;
        }
        
        #endregion C'tor


        #region Events and Events Handlig

        public event PropertyChangedEventHandler PropertyChanged;

        //Fire event when Welcome screen is displayed
        public event EventHandler DisplayWelcomeEvent;

        //Fire event when Contact screen is displayed
        public event EventHandler DisplayContactEvent;
        
        private void ChatClient_ConnectionLostEvent(object sender, EventArgs e)
        {
            Error = "Connection to server lost";
            UserControlToDisplay = ScreensEnum.Welcome;
        }

        private void ChatResult_loggedInEvent(object sender, EventArgs e)
        {
            ConnectedContacts = ((ContactListsEventArgs)e).ConnectedContacts;
            DisconnectedContacts = ((ContactListsEventArgs)e).DisconnectedContacts;
        }


        #endregion Events and Events Handlig
        

        #region Singleton Implementation

        private static MainVM _instance;
        public static MainVM Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MainVM();
                }
                return _instance;
            }
        }

        #endregion Singleton Implementation

        #region Private Methods

        private void FireChanegScreenEvent()
        {
            if (_controlToDisplay == ScreensEnum.Contact)
            {
                DisplayContactEvent?.Invoke(null, null);
            }
            else
            {
                DisplayWelcomeEvent?.Invoke(null, null);
            }
        }

        #endregion Private Methods
    }
}

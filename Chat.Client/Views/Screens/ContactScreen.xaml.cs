using Chat.Logic;
using Chat.UI.Views.Windows;
using Chat.UI.VM;
using Contracts;
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

namespace Chat.UI.Views.Screens
{
    /// <summary>
    /// Interaction logic for ContactScreen.xaml
    /// </summary>
    public partial class ContactScreen : UserControl
    {
        #region Members

        MainVM _mainVM;

        //Holds all the open chat windows
        Dictionary<string, ChatWindow> _openChatsList;

        //List of unaswered game request
        Dictionary<string, Guid?> _waitingForResponse;

        #endregion Members


        #region C'tor

        public ContactScreen()
        {
            InitializeComponent();
            _mainVM = MainVM.Instance;
            this.DataContext = _mainVM;
            ChatResult.ContactChangeEvent += ChatResult_contactChangeEvent;

            //Open chat when Send Message menu item clicked
            MainWindow.OpenChatEvent += MenuItem_OpenChatEvent;

            //Disconnect menu item clicked
            MainWindow.ConnectionEvent += Menu_DisonnectionEvent;

            //Message list from contact recieved
            ChatResult.MessagesListRecievedEvent += ChatResult_MessagesListRecievedEvent;

            //Game request recieved
            ChatResult.GameRequestedEvent += ChatResult_GameRequestedEvent;

            //Game response recieved
            ChatResult.GameRespondEvent += ChatResult_GameRespondEvent; ;

            //Play Backgammon menu item clicked
            MainWindow.PlayEvent += MenuItem_PlayEvent;

            //Game Ended recieved
            ChatResult.GameEndedEvent += ChatResult_GameEndedEvent;

            _openChatsList = new Dictionary<string, ChatWindow>();

            _waitingForResponse = new Dictionary<string, Guid?>();

        }




        #endregion C'tor


        #region Event Handling

        #region UI Commands Events

        //Load contact lists after user control loaded
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _mainVM.ConnectedContacts.Remove(_mainVM.UserName);
            _mainVM.DisconnectedContacts.Remove(_mainVM.UserName);
            online_lb.ItemsSource = _mainVM.ConnectedContacts;
            offline_lb.ItemsSource = _mainVM.DisconnectedContacts;
            ReportError(LogLevel.Information, $"Contact screen is displayed for user {_mainVM.UserName}");
        }

        //Disconnect menu item clicked
        private void Menu_DisonnectionEvent(object sender, EventArgs e)
        {
            try
            {
                bool isConnect = ((ConnectionEventArgs)e).Connect;
                if (!isConnect)
                {
                    Task.Run(new Action(() => Logoff()));
                    MainWindow.ConnectionEvent -= Menu_DisonnectionEvent;
                }
            }
            catch (Exception ex)
            {
                ReportError(LogLevel.Exception, ex.Message);
            }
        }

        //Open chat menu item was clicked
        private void MenuItem_OpenChatEvent(object sender, EventArgs e)
        {
            chat_b_Click(null, null);

        }

        //Open chat button was clicked
        private void chat_b_Click(object sender, RoutedEventArgs e)
        {
            string contact = (string)online_lb.SelectedItem;
            if (contact != null)
            {
                ActivateChat(contact);

            }

        }

        //Open game button clicked
        private void backgammon_b_Click(object sender, RoutedEventArgs e)
        {
            string contact = (string)online_lb.SelectedItem;
            if (contact != null)
            {
                //Chat or Game already open with contact
                if (_openChatsList.ContainsKey(contact))
                {
                    //Chat open with contact
                    if (!_openChatsList[contact].IsGame)
                    {
                        RequestGame(contact);
                    }
                }
                else
                {
                    RequestGame(contact);
                }

            }
        }

        //Chat window was closed
        private void Chat_WindowClosedEvent(object sender, EventArgs e)
        {
            string contact = ((ChatEventArgs)e).contact;
            Guid? session = ((ChatEventArgs)e).Session;
            if (session != null)
            {
                ChatClient.EndGame((Guid)session);
            }
            _openChatsList.Remove(contact);
        }

        //Play Backgammon menu item was clicked
        private void MenuItem_PlayEvent(object sender, EventArgs e)
        {
            backgammon_b_Click(null, null);
        }

        #endregion UI Commands Events

        #region Chat Result Events

        //Contact logged in/out
        private void ChatResult_contactChangeEvent(object sender, EventArgs e)
        {
            try
            {
                string contact = ((ContactChangeEventArgs)e).Contact;
                bool isConnected = ((ContactChangeEventArgs)e).IsConnected;

                if (isConnected)
                {
                    _mainVM.ConnectedContacts.Add(contact);
                    _mainVM.DisconnectedContacts.Remove(contact);
                }
                else
                {
                    _mainVM.ConnectedContacts.Remove(contact);
                    _mainVM.DisconnectedContacts.Add(contact);
                    if (_openChatsList.ContainsKey(contact))
                    {
                        Dispatcher.BeginInvoke(new System.Action(() =>
                        {
                            _openChatsList[contact].CloseChat();
                        }));
                        MessageBox.Show($"User {contact} disconnected");


                    }
                }
                Dispatcher.BeginInvoke(new System.Action(() =>
                {
                    online_lb.ItemsSource = _mainVM.ConnectedContacts;
                    offline_lb.ItemsSource = _mainVM.DisconnectedContacts;
                    online_lb.Items.Refresh();
                    offline_lb.Items.Refresh();
                }));
                ReportError(LogLevel.Information, $"Contact {contact} disconnected from user {_mainVM.UserName}");
            }
            catch (Exception ex)
            {
                ReportError(LogLevel.Exception, ex.Message);
            }

        }

        //A game request from a contact was recieved
        private async void ChatResult_GameRequestedEvent(object sender, EventArgs e)
        {
            string contact = ((ContactChangeEventArgs)e).Contact;
            Guid? session = ((ContactChangeEventArgs)e).Session;
            MessageBoxResult result = MessageBox.Show($"{contact} requested a game of backgammon. Do you agree?",
                                                      $"Game Request from {contact}", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                await ChatClient.RespondToGameRequest(_mainVM.UserName, contact, true);
                Dispatcher.BeginInvoke(new System.Action(() =>
                {
                    ActivateGame(contact, session);
                }));

            }
            else
            {
                await ChatClient.RespondToGameRequest(_mainVM.UserName, contact, false);
            }
        }

        private void ChatResult_GameRespondEvent(object sender, EventArgs e)
        {
            bool isAccepted = ((GameResponseEventArgs)e).IsAccepted;
            string contact = ((GameResponseEventArgs)e).Contact;
            if (isAccepted)
            {
                Dispatcher.BeginInvoke(new System.Action(() =>
                {
                    ActivateGame(contact,_waitingForResponse[contact]);
                    _waitingForResponse.Remove(contact);
                }));

            }
            else
            {
                _waitingForResponse.Remove(contact);
                MessageBox.Show($"{contact} declined the game request");
            }
        }

        //List of all the messages between user and contact was recieved
        private void ChatResult_MessagesListRecievedEvent(object sender, EventArgs e)
        {
            string from = ((MessageListsEventArgs)e).Sender;
            if (!_openChatsList.ContainsKey(from))
            {
                Dispatcher.BeginInvoke(new System.Action(() =>
                {
                    ActivateChat(from);
                }));
            }

        }

        private void ChatResult_GameEndedEvent(object sender, EventArgs e)
        {
            try
            {
                string contact = ((ContactChangeEventArgs)e).Contact;

                if (_openChatsList.ContainsKey(contact))
                {
                    Dispatcher.BeginInvoke(new System.Action(() =>
                    {
                        _openChatsList[contact].CloseChat();
                    }));
                    MessageBox.Show($"Game with {contact} ended");
                    _openChatsList.Remove(contact);

                }
            }
            catch (Exception ex)
            {
                ReportError(LogLevel.Exception, ex.Message);
            }
        }

        #endregion Chat Result Events


        #endregion Event Handling


        #region Private Methods

        private void Logoff()
        {
            ChatClient.logoff(_mainVM.UserName);
            _mainVM.IsWaiting = true;
            foreach (ChatWindow chat in _openChatsList.Values)
            {
                Dispatcher.BeginInvoke(new System.Action(() =>
                {
                    chat.CloseChat();
                }));
            }
            _mainVM.UserControlToDisplay = ScreensEnum.Welcome;
            ReportError(LogLevel.Information, $"User {_mainVM.UserName} disconnected");
        }

        private void ReportError(LogLevel level, string message)
        {
            Report.log(DeviceToReport.Client_ContactWindow, level, message);
        }

        private void ActivateChat(string contact)
        {
            //Chat window with contact is not open
            if (!_openChatsList.ContainsKey(contact))
            {
                Dispatcher.BeginInvoke(new System.Action(() =>
                {
                    OpenChatWindow(contact, false);

                }));

            }
            //Chat (or game) window already open
            else
            {
                _openChatsList[contact].Activate();
            }
        }

        private void RequestGame(string contact)
        {
            Guid? session = (Guid)ChatClient.RequestGame(_mainVM.UserName, contact);
            _waitingForResponse.Add(contact, session);
        }

        private void ActivateGame(string contact, Guid? session = null)
        {
            ChatWindow window;
            //Chat or game window with contact is not open
            if (!_openChatsList.ContainsKey(contact))
            {
                OpenChatWindow(contact, true, session);
            }
            //Chat or game already open
            else
            {
                window = _openChatsList[contact];
                //Chat window open
                if (!window.IsGame)
                {
                    window.Close();
                    _openChatsList.Remove(contact);
                    OpenChatWindow(contact, true, session);
                }

                //game is open 
                else
                {
                    _openChatsList[contact].Activate();
                }

            }
        }

        private void OpenChatWindow(string contact, bool isGame, Guid? session = null)
        {
            ChatWindow chat = new ChatWindow(contact, isGame,session);
            _openChatsList.Add(contact, chat);
            chat.Show();
            chat.WindowClosedEvent += Chat_WindowClosedEvent;
        }

        #endregion Private Methods


    }
}

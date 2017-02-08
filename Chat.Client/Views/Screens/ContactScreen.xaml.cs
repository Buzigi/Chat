﻿using Chat.Logic;
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

namespace Chat.UI.UserControls
{
    /// <summary>
    /// Interaction logic for ContactScreen.xaml
    /// </summary>
    public partial class ContactScreen : UserControl
    {
        #region Members

        MainVM _mainVM;

        //Holds all the open chat windows
        Dictionary<string,ChatWindow> _openChatsList;

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

            _openChatsList = new Dictionary<string, ChatWindow>();

        }


        #endregion C'tor


        #region Event Handling

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

        private void MenuItem_OpenChatEvent(object sender, EventArgs e)
        {
            chat_b_Click(null, null);

        }

        private void chat_b_Click(object sender, RoutedEventArgs e)
        {
            string contact = (string)online_lb.SelectedItem;
            if (contact != null)
            {
                ActivateChat(contact);

            }

        }

        private void backgammon_b_Click(object sender, RoutedEventArgs e)
        {
            string contact = (string)online_lb.SelectedItem;
            if (contact != null)
            {
                RequestGame(contact);
                //ActivateGame(contact);

            }
        }
        
        private void Chat_WindowClosedEvent(object sender, EventArgs e)
        {
            string contact = ((ChatEventArgs)e).contact;
            _openChatsList.Remove(contact);
        }

        private void ChatResult_GameRequestedEvent(object sender, EventArgs e)
        {
            string contact = ((ContactChangeEventArgs)e).Contact;
            MessageBoxResult result = MessageBox.Show($"{contact} requested a game of backgammon. Do you agree?", 
                                                      $"Game Request from {contact}", MessageBoxButton.YesNo);
        }

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
            if (!_openChatsList.ContainsKey(contact))
            {
                ChatWindow chat = new ChatWindow(contact);
                _openChatsList.Add(contact, chat);
                chat.Show();
                chat.WindowClosedEvent += Chat_WindowClosedEvent;
            }
            //Chat already open
            else
            {
                _openChatsList[contact].Activate();
            }
        }

        private void RequestGame(string contact)
        {
            ChatClient.RequestGame(_mainVM.UserName, contact);
        }

        private void ActivateGame(string contact)
        {
            if (!_openGamesList.ContainsKey(contact))
            {
                Backgammon.UI.MainWindow game = new Backgammon.UI.MainWindow(_mainVM.UserName,contact);
                _openGamesList.Add(contact, game);
                game.Show();
                //game.WindowClosedEvent += Chat_WindowClosedEvent;
            }
            //Chat already open
            else
            {
                _openGamesList[contact].Activate();
            }
        }
        #endregion Private Methods


    }
}
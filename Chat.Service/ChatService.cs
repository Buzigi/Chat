using Contracts;
using Chat.DAL;
using Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Chat.Contracts;

namespace Service
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single, InstanceContextMode = InstanceContextMode.Single)]
    public class ChatService : IChatService
    {
        #region Fields

        private Dictionary<Guid, IChatResult> _sessions;
        private Dictionary<string, IChatResult> _clients;
        private Dictionary<Guid, string[]> _sessionDetails;

        public static event EventHandler ReportEvent;

        private delegate void DoWorkDelegate(string sender, string reciever);


        #endregion Fields


        #region C'tor

        public ChatService()
        {
            _sessions = new Dictionary<Guid, IChatResult>();
            _sessionDetails = new Dictionary<Guid, string[]>();
            _clients = new Dictionary<string, IChatResult>(StringComparer.OrdinalIgnoreCase);
            List<string> clients = ChatDBAccess.GetAllUsers();
            foreach (string client in clients)
            {
                _clients.Add(client, null);
            }
        }

        #endregion C'tor


        #region IChatService Implementation

        public LoginError Login(string userName, string password)
        //Returns true if userName exists in DB and the password is correct, otherwise returns False
        //If user logs in correctly it is added to logged users dictionary and all other connected users are notified
        //Upon connection user recieves a connected users list
        {
            try
            {
                //userName is in DB
                if (_clients.ContainsKey(userName))
                {
                    //userName is not logged in 
                    if (_clients[userName] == null)
                    {
                        //password is correct for userName
                        if (ChatDBAccess.IsPasswordCorrect(userName, password))
                        {
                            IChatResult callback = OperationContext.Current.GetCallbackChannel<IChatResult>();
                            NotifyLoggedClients(userName, true);
                            _clients[userName] = callback;
                            Dictionary<string, bool> contacts = new Dictionary<string, bool>();
                            foreach (string client in _clients.Keys)
                            {
                                if (_clients[client] != null)
                                {
                                    contacts.Add(client, true);
                                }
                                else
                                {
                                    contacts.Add(client, false);
                                }
                            }

                            callback.ConnectedContactsData(contacts);
                            Report($"User {userName} logged in", LogLevel.Information);
                            return LoginError.NoError;
                        }
                        else
                        {
                            Report($"Incorrect password for user {userName}", LogLevel.Information);
                            return LoginError.WrongPassword;
                        }
                    }
                    else
                    {
                        Report($"User {userName} is already connected to the server", LogLevel.Information);
                        return LoginError.UserAlreadyConnected;
                    }
                }
                else
                {
                    Report($"{userName} does not exist in DB", LogLevel.Information);
                    return LoginError.UsernameDoesNotExist;
                }
            }
            catch (Exception e)
            {
                Report(e.Message, LogLevel.Exception);
                return LoginError.UnknownException;
            }
        }

        public bool Register(string userName, string password)
        //Add user to DB and log in
        {
            try
            {
                if (ChatDBAccess.AddUser(userName, password))
                {
                    Report($"User {userName} added to DB", LogLevel.Information);
                    _clients.Add(userName, null);
                    Login(userName, password);
                    return true;
                }
                else
                {
                    Report($"User {userName} already exists in DB. Please choose another name", LogLevel.Information);
                    return false;
                }
            }
            catch (Exception e)
            {
                Report(e.Message, LogLevel.Exception);
                return false;
            }
        }

        public void LogOut(string userName)
        //Logs userName out of the server
        {
            try
            {
                _clients[userName] = null;
                Report($"user {userName} logged out", LogLevel.Information);
                NotifyLoggedClients(userName, false);
            }
            catch (Exception e)
            {
                Report(e.Message, LogLevel.Exception);
            }
        }

        public bool IsActive()
        {
            return true;
        }

        public Guid? Chat(string sender, string reciever)
        //Adds a chat session to reciever
        {
            try
            {
                DoWorkDelegate del = SendMessagesListToClient;
                return OpenSession(sender, reciever, del);
            }
            catch (Exception e)
            {
                Report($"Could not open a chat with {reciever}", LogLevel.Exception);
                Report(e.Message, LogLevel.Exception);
                return null;
            }

        }

        public void EndChat(Guid session)
        //Ends a chat session with userName
        {
            try
            {
                _sessions.Remove(session);
                _sessionDetails.Remove(session);
                Report($"Session {session} ended", LogLevel.Information);
            }
            catch (Exception e)
            {
                Report($"Session {session} was not ended", LogLevel.Exception);
                Report(e.Message, LogLevel.Exception);
            }
        }

        public bool SendMessage(Guid session, string text)
        {
            try
            {
                //Reciever still online
                if (_sessions.ContainsKey(session))
                {
                    string sender = _sessionDetails[session][0];
                    string reciever = _sessionDetails[session][1];
                    Message message = new Message()
                    {
                        Receiver = _sessionDetails[session][1],
                        Sender = _sessionDetails[session][0],
                        SentTime = DateTime.Now,
                        Text = text

                    };
                    ChatDBAccess.AddMessage(message);
                    Report($"Message fron {sender} to {reciever} added to DB", LogLevel.Information);
                    SendMessagesListToClient(sender, reciever);
                    SendMessagesListToClient(reciever, sender);
                    Report($"Message recieved by {reciever}", LogLevel.Information);
                    return true;

                }
                //Reciever offline
                {
                    Report($"Message was not recieved - reciever offline", LogLevel.Warning);
                    return false;
                }
            }
            catch (Exception e)
            {
                Report($"Message was not sent", LogLevel.Exception);
                Report(e.Message, LogLevel.Exception);
                return false;
            }
        }

        #endregion IChatService Implementation


        #region IBackgammonService Implementation


        public Guid? RequestNewGame(string sender, string reciever)
        {
            try
            {
                DoWorkDelegate del = SendGameToClient;
                Report($"Game request sent to {reciever}", LogLevel.Information);
                return OpenSession(sender, reciever, del);
            }
            catch (Exception e)
            {
                Report($"Could not open a game with {reciever}", LogLevel.Exception);
                Report(e.Message, LogLevel.Exception);
                return null;
            }
        }

        public void EndGame(Guid session)
        //Ends a game session with userName
        {
            try
            {
                _sessions[session].GameEnded(_sessionDetails[session][0]);
                _sessions.Remove(session);
                _sessionDetails.Remove(session);
                Report($"Game {session} ended", LogLevel.Information);
            }
            catch (Exception e)
            {
                Report($"Game {session} was not ended", LogLevel.Exception);
                Report(e.Message, LogLevel.Exception);
            }
        }

        public bool RestartGame(Guid session)
        {
            throw new NotImplementedException();
        }

        public Guid? AcceptGame(string userName, string contact, bool isAccepted)
        {
            try
            {
                if (isAccepted)
                {
                    DoWorkDelegate del = SendGameResponseToClient;
                    Report($"Game response sent to {contact}", LogLevel.Information);
                    return OpenSession(userName, contact, del);
                }
                else
                {
                    _clients[contact].NewGameResponse(userName, false);
                    return null;
                }
            }
            catch (Exception e)
            {
                Report($"Could not send response to {contact}", LogLevel.Exception);
                Report(e.Message, LogLevel.Exception);
                return null;
            }
        }

        public void SendMove(Guid session, List<Move> moves)
        {
            try
            {
                //Reciever still online
                if (_sessions.ContainsKey(session))
                {
                    string reciever = _sessionDetails[session][1];
                    string sender = _sessionDetails[session][0];
                    _sessions[session].GetMove(moves,sender);
                    Report($"Move recieved by {reciever}", LogLevel.Information);

                }
                //Reciever offline
                {
                    Report($"Move was not recieved - reciever offline", LogLevel.Warning);
                }
            }
            catch (Exception e)
            {
                Report($"Move was not sent", LogLevel.Exception);
                Report(e.Message, LogLevel.Exception);
            }
        }

        #endregion IBackgammonService Implementation


        #region Private Methods

        private void Report(string message, LogLevel severity)
        //transmit server messages to host
        {
            Helpers.Report.log(DeviceToReport.Service, severity, message);
            ReportEvent?.Invoke(this, new ChatServiceEventArgs(message));

        }

        private void NotifyLoggedClients(string userName, bool isConnected)
        // when {userName} logs into server, all other logged users are notified
        {
            try
            {
                foreach (KeyValuePair<string, IChatResult> client in _clients)
                {
                    if (client.Value != null)
                    {
                        client.Value.ContactData(userName, isConnected);
                    }
                }
                string connectionStatus;
                if (isConnected)
                {
                    connectionStatus = "logged in";
                }
                else
                {
                    connectionStatus = "logged out";
                }
                Report($"All logged users were notified {userName} {connectionStatus}", LogLevel.Information);

            }
            catch (Exception e)
            {
                Report(e.Message, LogLevel.Exception);
            }
        }

        private void SendMessagesListToClient(string sender, string reciever)
        {
            List<Message> messages = ChatDBAccess.GetListOfMessages(sender, reciever);
            _clients[sender].GetMessageList(messages, reciever);
        }

        private void SendGameToClient(string sender, string reciever)
        {
            Guid session = Guid.NewGuid();
            _sessions.Add(session, _clients[sender]);
            _sessionDetails.Add(session, new string[] { reciever, sender });
            _clients[reciever].GameRequested(sender, session);
        }

        private void SendGameResponseToClient(string sender, string reciever)
        {
            _clients[reciever].NewGameResponse(sender, true);
        }

        private Guid? OpenSession(string sender, string reciever, DoWorkDelegate del)
        {
            if (_clients.ContainsKey(reciever))
            //reciever is logged in
            {
                Guid session = Guid.NewGuid();
                _sessions.Add(session, _clients[reciever]);
                _sessionDetails.Add(session, new string[] { sender, reciever });
                Report($"Session {session} with {reciever} added", LogLevel.Information);
                del(sender, reciever);
                return session;
            }
            else
            //reciever is offline
            {
                Report($"{reciever} is offline", LogLevel.Information);
                return null;
            }
        }

        #endregion Private Methods

    }

    public class ChatServiceEventArgs : EventArgs
    {
        public string Message { get; set; }
        public ChatServiceEventArgs(string message)
        {
            Message = message;
        }
    }

}

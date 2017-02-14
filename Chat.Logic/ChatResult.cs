using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Logic
{
    public class ChatResult : IChatResult
    {

        #region Properties and Members


        public static event EventHandler ContactChangeEvent;

        public static event EventHandler LoggedInEvent;

        public static event EventHandler MessagesListRecievedEvent;

        public static event EventHandler GameRequestedEvent;

        public static event EventHandler GameRespondEvent;

        #endregion Properties and Members


        #region Chat Implementation

        //Recieves a list of contacts and their connection status from the server
        public void ConnectedContactsData(Dictionary<string, bool> contacts)
        {
            ContactListsEventArgs listsArgs = new ContactListsEventArgs();
            foreach (KeyValuePair<string, bool> contact in contacts)
            {
                //User online
                if (contact.Value)
                {
                    listsArgs.ConnectedContacts.Add(contact.Key);
                }

                //User offline
                else
                {
                    listsArgs.DisconnectedContacts.Add(contact.Key);
                }
            }
            LoggedInEvent?.Invoke(null, listsArgs);
        }

        //Recieves a connection status of a specific contact from the server
        public void ContactData(string contactName, bool isConnected)
        {
            ContactChangeEventArgs contactArgs = new ContactChangeEventArgs()
            {
                Contact = contactName,
                IsConnected = isConnected
            };
            ContactChangeEvent?.Invoke(this, contactArgs);
        }

        #endregion Chat Implementation


        #region Backgammon Implementation

        public void GameEnded(string contact)
        {
            throw new NotImplementedException();
        }

        //Recieved a new game request
        public void GameRequested(string contact)
        {
            GameRequestedEvent?.Invoke(null, new ContactChangeEventArgs() { Contact = contact });
        }

        //Recieves a list of messages between this user and a contact
        public void GetMessageList(List<Message> messages, string reciever)
        {
            MessagesListRecievedEvent?.Invoke(null, new MessageListsEventArgs(messages, reciever));
        }

        public void GetMove(string contact, int piece, int moves)
        {
            throw new NotImplementedException();
        }

        public void NewGameResponse(string contact, bool isAccepted)
        {
            GameRespondEvent?.Invoke(null, new GameResponseEventArgs(contact, isAccepted));
        }

        #endregion Backgammon Implementation
    }

    public class ContactChangeEventArgs : EventArgs
    {
        public string Contact { get; set; }
        public bool IsConnected { get; set; }
    }

    public class ContactListsEventArgs : EventArgs
    {
        public List<string> ConnectedContacts { get; set; }
        public List<string> DisconnectedContacts { get; set; }

        public ContactListsEventArgs()
        {
            ConnectedContacts = new List<string>();
            DisconnectedContacts = new List<string>();
        }
    }

    public class MessageListsEventArgs : EventArgs
    {
        public List<Message> Messages { get; set; }

        public string Sender { get; set; }

        public MessageListsEventArgs(List<Message> messages, string sender)
        {
            Messages = messages;
            Sender = sender;
        }
    }

    public class GameResponseEventArgs : EventArgs
    {
        public string Contact { get; set; }
        public bool IsAccepted { get; set; }

        public GameResponseEventArgs(string contact, bool isAccepted)
        {
            Contact = contact;
            IsAccepted = isAccepted;
        }
    }
}

using Chat.Logic;
using Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.UI.VM
{
    public class ChatVM: INotifyPropertyChanged
    {
        #region Properties and Private Fields

        public static string Sender { get; set; }

        public string Reciever { get; set; }

        public Guid? Session { get; set; }

        //All of the messages between Sender and Reciever
        public List<Message> Messages { get; set; }

        public ChatVM _instance;

        #endregion Properties and Private Fields


        #region C'tor

        public ChatVM(string sender, string reciever)
        {
            _instance = this;
            Sender = sender;
            Reciever = reciever;
            ChatResult.MessagesListRecievedEvent += ChatResult_MessagesListRecievedEvent;
        }




        #endregion C'tor


        #region Events


        public event PropertyChangedEventHandler PropertyChanged;

        private void ChatResult_MessagesListRecievedEvent(object sender, EventArgs e)
        {
            Messages = ((MessageListsEventArgs)e).Messages;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Messages"));
        }

        #endregion Events


        #region Public Methods

        public void StartChat()
        {
            Session = ChatClient.AddSession(Sender, Reciever);
        }

        public static string GetSender()
        {
            return Sender;
        }

        #endregion Public Methods
    }
}

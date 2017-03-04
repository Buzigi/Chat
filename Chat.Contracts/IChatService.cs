using Chat.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    [ServiceContract(CallbackContract = typeof(IChatResult))]
    public interface IChatService
    {
        #region Chat

        [OperationContract]
        LoginError Login(string userName, string password);

        [OperationContract]
        bool Register(string userName, string password);

        [OperationContract(IsOneWay = true)]
        void LogOut(string userName);

        [OperationContract]
        bool IsActive();

        [OperationContract]
        Guid? Chat(string sender, string reciever);

        [OperationContract(IsOneWay = true)]
        void EndChat(Guid session);

        [OperationContract]
        bool SendMessage(Guid session, string message);

        #endregion Chat

        #region Game

        [OperationContract]
        Guid? RequestNewGame(string userName, string contact);

        [OperationContract]
        Guid? AcceptGame(string userName, string contact, bool isAccepted);

        [OperationContract(IsOneWay = true)]
        void EndGame(Guid session);

        [OperationContract(IsOneWay =true)]
        void SendMove(Guid session, List<Move> moves);

        [OperationContract]
        bool RestartGame(Guid session);

        #endregion Game

    }

    [DataContract]
    public enum LoginError
    {
        [EnumMember]
        NoError,
        [EnumMember]
        UsernameDoesNotExist,
        [EnumMember]
        WrongPassword,
        [EnumMember]
        UserAlreadyConnected,
        [EnumMember]
        UnknownException
    }

}

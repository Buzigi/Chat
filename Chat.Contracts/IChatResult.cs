using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    [ServiceContract]
    public interface IChatResult
    {
        #region Chat

        [OperationContract(IsOneWay =true)]
        void ConnectedContactsData(Dictionary<string,bool> contacts);

        [OperationContract(IsOneWay =true)]
        void ContactData(string contactName, bool isConnected);

        [OperationContract(IsOneWay = true)]
        void GetMessageList(List<Message> messages, string sender);

        #endregion Chat

        #region Game

        [OperationContract(IsOneWay = true)]
        void NewGameResponse(string contact, bool isAccepted);

        [OperationContract(IsOneWay = true)]
        void GameRequested(string contact, Guid? session);

        [OperationContract(IsOneWay = true)]
        void GetMove(int piece, int moves);

        [OperationContract(IsOneWay = true)]
        void GameEnded(string contact);

        #endregion Game
    }
}

using System.Collections.Generic;
using System.ServiceModel;
using WcfChatHost.Models;

namespace WcfChatHost
{
    [ServiceContract]
    public interface IChatClientCallback
    {
        [OperationContract(IsOneWay = true)]
        void RefreshUsers(List<User> users);

        [OperationContract(IsOneWay = true)]
        void Receive(ChatMessage msg);

        [OperationContract(IsOneWay = true)]
        void ReceiveWhisper(ChatMessage msg, User receiver);

        [OperationContract(IsOneWay = true)]
        void IsWritingCallback(User user);

        [OperationContract(IsOneWay = true)]
        void UserJoin(User user);

        [OperationContract(IsOneWay = true)]
        void UserLeave(User user);
    }
}

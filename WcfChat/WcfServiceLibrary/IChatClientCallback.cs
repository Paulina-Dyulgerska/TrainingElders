using System.Collections.Generic;
using System.ServiceModel;
using WcfServiceLibrary.Models;

namespace WcfServiceLibrary
{
    [ServiceContract]
    public interface IChatClientCallback
    {
        [OperationContract(IsOneWay = true)]
        void RefreshUsers(IEnumerable<User> users);

        [OperationContract(IsOneWay = true)]
        void Receive(ChatMessage msg);

        [OperationContract(IsOneWay = true)]
        void UserJoin(User user);

        [OperationContract(IsOneWay = true)]
        void UserLeave(User user);
    }
}

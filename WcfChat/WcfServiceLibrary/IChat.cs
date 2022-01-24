using System.ServiceModel;
using WcfServiceLibrary.Models;

namespace WcfServiceLibrary
{
    [ServiceContract(CallbackContract = typeof(IChatClientCallback))]
    public interface IChat
    {
        [OperationContract()] //IsOneWay = true
        bool Connect(User user);

        [OperationContract(IsOneWay = true)]
        void Say(ChatMessage msg);

        [OperationContract(IsOneWay = true)]
        void Disconnect(User user);
    }
}

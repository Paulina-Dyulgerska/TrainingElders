using System.ServiceModel;
using WcfChatHost.Models;

namespace WcfChatHost
{
    [ServiceContract(CallbackContract = typeof(IChatClientCallback))]
    public interface IChat
    {
        [OperationContract()] //IsOneWay = true
        bool Connect(User user);

        [OperationContract(IsOneWay = true)]
        void Say(ChatMessage msg);

        [OperationContract(IsOneWay = true)]
        void Whisper(ChatMessage msg, User receiver);

        [OperationContract(IsOneWay = true)]
        void IsWriting(User user);

        [OperationContract(IsOneWay = true)]
        void Disconnect(User user);
    }
}

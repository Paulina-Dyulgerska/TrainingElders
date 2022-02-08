using System;
using System.Threading.Tasks;

namespace ChatModels
{
    public interface IChatMessageReceiver
    {
        void Start(Client client);

        void Stop();

        void RegisterMessageReseivedHandler(Func<Client, ChatMessage, bool> func);

        void RegisterClientConnectedHandler(Func<Client, Task> func);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WcfChatHost.Models;

namespace WcfChatHost
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
                           ConcurrencyMode = ConcurrencyMode.Multiple,
                           UseSynchronizationContext = false)]
    //ConcurrencyMode.Single
    public class Chat : IChat
    {
        private readonly Dictionary<User, IChatClientCallback> Users = new Dictionary<User, IChatClientCallback>();
        private readonly List<User> UserList = new List<User>();

        object syncObj = new object();

        public IChatClientCallback CurrentCallback
        {
            get
            {
                return OperationContext.Current.GetCallbackChannel<IChatClientCallback>();

            }
        }

        private bool SearchUsersByName(string name)
        {
            foreach (User c in Users.Keys)
            {
                if (c.Name == name)
                {
                    return true;
                }
            }
            return false;
        }

        public bool Connect(User User)
        {
            if (!Users.ContainsValue(CurrentCallback) && !SearchUsersByName(User.Name))
            {
                lock (syncObj)
                {
                    Users.Add(User, CurrentCallback);
                    UserList.Add(User);

                    foreach (User key in Users.Keys)
                    {
                        IChatClientCallback callback = Users[key];
                        try
                        {
                            callback.RefreshUsers(UserList);
                            callback.UserJoin(User);
                        }
                        catch
                        {
                            Users.Remove(key);
                            return false;
                        }
                    }

                }
                return true;
            }
            return false;
        }

        public void Say(ChatMessage msg)
        {
            lock (syncObj)
            {
                foreach (IChatClientCallback callback in Users.Values)
                {
                    callback.Receive(msg);
                }
            }
        }

        public void Whisper(ChatMessage msg, User receiver)
        {
            foreach (User rec in Users.Keys)
            {
                if (rec.Name == receiver.Name)
                {
                    IChatClientCallback callback = Users[rec];
                    callback.ReceiveWhisper(msg, rec);

                    foreach (User sender in Users.Keys)
                    {
                        if (sender.Name == msg.Sender)
                        {
                            IChatClientCallback senderCallback = Users[sender];
                            senderCallback.ReceiveWhisper(msg, rec);
                            return;
                        }
                    }
                }
            }
        }

        public void IsWriting(User User)
        {
            lock (syncObj)
            {
                foreach (IChatClientCallback callback in Users.Values)
                {
                    callback.IsWritingCallback(User);
                }
            }
        }

        public void Disconnect(User User)
        {
            foreach (User c in Users.Keys)
            {
                if (User.Name == c.Name)
                {
                    lock (syncObj)
                    {
                        this.Users.Remove(c);
                        this.UserList.Remove(c);
                        foreach (IChatClientCallback callback in Users.Values)
                        {
                            callback.RefreshUsers(this.UserList);
                            callback.UserLeave(User);
                        }
                    }
                    return;
                }
            }
        }
    }
}

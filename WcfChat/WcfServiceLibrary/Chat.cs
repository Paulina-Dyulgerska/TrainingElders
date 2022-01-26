using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using WcfServiceLibrary.Models;

namespace WcfServiceLibrary
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class Chat : IChat
    {
        static readonly object mutex = new object();
        private readonly ConcurrentDictionary<User, (IContextChannel channel, IChatClientCallback callback)> Users = new ConcurrentDictionary<User, (IContextChannel channel, IChatClientCallback callback)>();
        public IEnumerable<User> AllUsers => Users.Keys;

        private void OnChannelFaulted(object sender, EventArgs e)
        {
            var kvp = Users.Where(x => x.Value.callback == sender).FirstOrDefault();
            var user = kvp.Key;
            if (user != null)
            {
                Disconnect(user);
                kvp.Value.channel.Faulted -= OnChannelFaulted;
            }
        }

        public bool Connect(User user)
        {
            if (AllUsers.Any(x => x.Name == user.Name))
                return false;

            lock (mutex)
            {
                Users.TryAdd(user, (OperationContext.Current.Channel, OperationContext.Current.GetCallbackChannel<IChatClientCallback>()));

                OperationContext.Current.Channel.Faulted += OnChannelFaulted;

                foreach (User key in AllUsers)
                {
                    var (_, callback) = Users[key];
                    try
                    {
                        callback.RefreshUsers(AllUsers);
                        callback.UserJoin(user);
                    }
                    catch
                    {
                        Users.TryRemove(key, out var _);
                        return false;
                    }
                }
            }

            return true;
        }

        public void Say(ChatMessage msg)
        {
            lock (mutex)
            {
                foreach (var callback in Users.Values.Select(x => x.callback))
                {
                    callback.Receive(msg);
                }
            }
        }

        public void Disconnect(User user)
        {
            foreach (User c in AllUsers)
            {
                if (user.Name == c.Name)
                {
                    lock (mutex)
                    {
                        Users.TryRemove(c, out var _);
                        foreach (var callback in Users.Values.Select(x => x.callback))
                        {
                            callback.UserLeave(user);
                            callback.RefreshUsers(AllUsers);
                        }
                    }

                    return;
                }
            }
        }
    }
}

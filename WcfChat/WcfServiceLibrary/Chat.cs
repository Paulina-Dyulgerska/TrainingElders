using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using WcfServiceLibrary.Models;

namespace WcfServiceLibrary
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
                              ConcurrencyMode = ConcurrencyMode.Multiple,
                              UseSynchronizationContext = false)]
    //ConcurrencyMode.Single
    public class Chat : IChat
    {
        private readonly ConcurrentDictionary<User, IChatClientCallback> Users = new ConcurrentDictionary<User, IChatClientCallback>();
        public IEnumerable<User> AllUsers => Users.Keys;

        static readonly object mutex = new object();

        public IChatClientCallback CurrentCallback
        {
            get
            {
                OperationContext.Current.Channel.Faulted += new EventHandler(OnChannelFaulted);
                //OperationContext.Current.Channel.Closed += new EventHandler(OnChannelFaulted);
                return OperationContext.Current.GetCallbackChannel<IChatClientCallback>();

            }
        }

        private bool SearchUsersByName(string name)
        {
            foreach (User c in AllUsers)
            {
                if (c.Name == name)
                {
                    return true;
                }
            }
            return false;
        }

        private void OnChannelFaulted(object sender, EventArgs e)
        {
            //Console.WriteLine((IChatClientCallback)sender);

            var user = Users.Where(x => x.Value == (IChatClientCallback)sender).FirstOrDefault().Key;
            if (user != null)
            {
                //Console.WriteLine(String.Join(", ", UserList.Select(x => x.Name)));
                //Console.WriteLine(user.Name + " - I am with a broken connection");
                //Users.Remove(user);
                //UserList.Remove(user);

                Disconnect(user);
            }
            //Console.WriteLine(String.Join(", ", UserList.Select(x => x.Name)));
        }

        public bool Connect(User user)
        {
            if (SearchUsersByName(user.Name) == false)
            {
                lock (mutex)
                {
                    Users.TryAdd(user, CurrentCallback);
                    //UserList.Add(user);

                    foreach (User key in AllUsers)
                    {
                        IChatClientCallback callback = Users[key];
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
            return false;
        }

        public void Say(ChatMessage msg)
        {
            lock (mutex)
            {
                foreach (IChatClientCallback callback in Users.Values)
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
                        foreach (IChatClientCallback callback in Users.Values)
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

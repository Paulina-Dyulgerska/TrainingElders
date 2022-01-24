using System;
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
        private readonly Dictionary<User, IChatClientCallback> Users = new Dictionary<User, IChatClientCallback>();
        private readonly List<User> UserList = new List<User>();

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
            foreach (User c in Users.Keys)
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

        public bool Connect(User User)
        {
            if (!Users.ContainsValue(CurrentCallback) && !SearchUsersByName(User.Name))
            {
                lock (mutex)
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
            lock (mutex)
            {
                foreach (IChatClientCallback callback in Users.Values)
                {
                    callback.Receive(msg);
                }
            }
        }

        public void Disconnect(User User)
        {
            foreach (User c in Users.Keys)
            {
                if (User.Name == c.Name)
                {
                    lock (mutex)
                    {
                        Users.Remove(c);
                        UserList.Remove(c);
                        foreach (IChatClientCallback callback in Users.Values)
                        {
                            callback.UserLeave(User);
                            callback.RefreshUsers(UserList);
                        }
                    }
                    return;
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace wcf_chat
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)] 
    public class ServiceChat : IServiceChat
    {
        private List<ServeUser> users = new List<ServeUser>();
        private int nextId = 1;

        public int Connect(string name)
        {
            ServeUser user = new ServeUser()
            {
                Id = nextId,
                Name = name,
                OperationContext = OperationContext.Current
            };
            nextId++;

            SendMsg(user.Name + "connect", 0);
            users.Add(user);
            return user.Id;
        }

        public void Disconnect(int id)
        {
            var user = users.FirstOrDefault(i => i.Id == id);
            if (user != null)
            {
                users.Remove(user);
                SendMsg(user.Name + "disconnect", 0);
            }
        }

        public void SendMsg(string msg, int id)
        {
            foreach (var item in users)
            {
                string answer = DateTime.Now.ToShortTimeString();
                var user = users.FirstOrDefault(i => i.Id == id);
                if (user != null)
                {
                    answer += ": " + user.Name + " ";
                }

                answer += msg;

                item.OperationContext.GetCallbackChannel<IServerChatCallback>().MsgCallback(answer);
            }
            
        }
    }
}

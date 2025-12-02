using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalnyy.BLL.Events
{

    public class ChatHubEvent
    {
        public delegate Task UserConnectedDel(string userId, string connectionId);
        public delegate Task UserDisconnectedDel(string connectionId);
        public delegate Task SendMessageDel(string senderId, string receiverId, string message, string senderConnectionId);
        public event SendMessageDel? SendMessage;


        public event UserConnectedDel? UserConnected;
        public event UserDisconnectedDel? UserDisconnected;
        public async Task FireUserConnectedAsync(string userId, string connectionId)
        {
            if (UserConnected != null)
                await UserConnected.Invoke(userId, connectionId);
        }

        public async Task FireOnUserDisconnectedAsync(string connectionId)
        {
            if (UserDisconnected != null)
                await UserDisconnected.Invoke(connectionId);
        }

        public async Task FireSendMessageAsync(string senderId, string receiverId, string message, string senderConnectionId)
        {
            if (SendMessage != null)
                await SendMessage.Invoke(senderId, receiverId, message, senderConnectionId);
        }
    }
}

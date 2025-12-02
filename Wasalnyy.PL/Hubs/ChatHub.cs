using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Wasalnyy.BLL.Events;

namespace Wasalnyy.PL.Hubs
{
   [Authorize(Roles = "Driver,Rider")]

    public class ChatHub : Hub
    {
        private readonly ChatHubEvent chatHubEvent;

        public ChatHub(ChatHubEvent chatHubEvent)
        {
            this.chatHubEvent = chatHubEvent;
        }
        public async Task sendmessage(string receiverId, string message)
        {
            var senderId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(senderId) || string.IsNullOrEmpty(receiverId) || string.IsNullOrEmpty(message))
                throw new HubException("Invalid message data. Sender, receiver, and message cannot be empty.");

            await chatHubEvent.FireSendMessageAsync(senderId, receiverId, message,Context.ConnectionId);
        }
        public override async Task OnConnectedAsync()
        {
            var currentUserId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string conId = Context.ConnectionId;

            if (string.IsNullOrEmpty(currentUserId) || string.IsNullOrEmpty(conId))
                throw new UnauthorizedAccessException();

            await base.OnConnectedAsync();

            await chatHubEvent.FireUserConnectedAsync(currentUserId, conId);
            Console.WriteLine("user " + currentUserId + " connected");
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string conId = Context.ConnectionId;

            if (!string.IsNullOrEmpty(conId))
                await chatHubEvent.FireOnUserDisconnectedAsync(conId);

            await base.OnDisconnectedAsync(exception);
        }
    }
}

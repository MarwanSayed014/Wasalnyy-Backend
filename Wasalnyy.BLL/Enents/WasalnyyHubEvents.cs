using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.DAL.Entities;

namespace Wasalnyy.BLL.Enents
{
    public class WasalnyyHubEvents
    {
        public delegate Task UserConnectedDel(string userId, string connectionId);
        public delegate Task UserDisconnectedDel(string connectionId);


        public event UserConnectedDel? UserConnected;
        public event UserDisconnectedDel? UserDisconnected;

        public async Task FireUserConnectedAsync(string userId, string connectionId)
        {
            if(UserConnected != null)
                await UserConnected.Invoke(userId, connectionId);
        }

        public async Task FireOnUserDisconnectedAsync(string connectionId)
        {
            if(UserDisconnected != null)
                await UserDisconnected.Invoke(connectionId);
        }
    }
}

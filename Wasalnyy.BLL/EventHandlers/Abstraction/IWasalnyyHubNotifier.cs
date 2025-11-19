using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalnyy.BLL.EventHandlers.Abstraction
{
    public interface IWasalnyyHubNotifier
    {
        Task OnUserConnected(string userId, string connectionId);
        Task OnUserDisconnected(string connectionId);

    }
}

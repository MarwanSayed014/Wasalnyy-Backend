using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.BLL.Service.Abstraction;

namespace Wasalnyy.BLL.Service.Implementation
{
    public class DriverService : IDriverService
    {
        public event IDriverService.DriverAvailableDel? DeriverAvailable;
        public event IDriverService.DriverLocationUpdatedDel? DriverLocationUpdated;

        public Task ToggleStatusAsync(string driverId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateLocationAsync(string driverId, decimal lng, decimal lat)
        {
            throw new NotImplementedException();
        }
    }
}

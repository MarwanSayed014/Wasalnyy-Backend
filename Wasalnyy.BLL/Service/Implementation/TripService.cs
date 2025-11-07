using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.BLL.DTO.Trip;
using Wasalnyy.BLL.Service.Abstraction;
using Wasalnyy.DAL.Entities;

namespace Wasalnyy.BLL.Service.Implementation
{
    public class TripService : ITripService
    {
        public event ITripService.TripDel? TripRequested;
        public event ITripService.TripDel? TripAccepted;
        public event ITripService.TripDel? TripStarted;
        public event ITripService.TripDel? TripEnded;
        public event ITripService.TripDel? TripCanceled;

        public Task AcceptTripAsync(Guid tripId)
        {
            throw new NotImplementedException();
        }

        public Task CancelTripAsync(Guid tripId)
        {
            throw new NotImplementedException();
        }

        public Task EndTripAsync(Guid tripId)
        {
            throw new NotImplementedException();
        }

        public Task<TripPaginationDto> GetAllAsync(Expression<Func<Trip, object>> orderBy, bool descending = false, int pageNumber = 1, int pageSize = 10)
        {
            throw new NotImplementedException();
        }

        public Task<TripPaginationDto> GetAllAsync(string riderId, Expression<Func<Trip, object>> orderBy, bool descending = false, int pageNumber = 1, int pageSize = 10)
        {
            throw new NotImplementedException();
        }

        public Task<TripDto> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TripDto>> GetByRequestedTripsByZoneAsync(string zone)
        {
            throw new NotImplementedException();
        }

        public Task RequestTripAsync(RequestTripDto dto)
        {
            throw new NotImplementedException();
        }
    }
}

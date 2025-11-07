using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.BLL.DTO.Trip;
using Wasalnyy.DAL.Entities;

namespace Wasalnyy.BLL.Service.Abstraction
{
    public interface ITripService
    {
        delegate void TripDel(TripDto dto);
        event TripDel? TripRequested;
        event TripDel? TripAccepted;
        event TripDel? TripStarted;
        event TripDel? TripEnded;
        event TripDel? TripCanceled;

        Task<TripDto> GetByIdAsync(Guid id);
        Task<IEnumerable<TripDto>> GetByRequestedTripsByZoneAsync(string zone);
        Task<TripPaginationDto> GetAllAsync(Expression<Func<Trip, object>> orderBy,
                                        bool descending = false, int pageNumber = 1, int pageSize = 10);
        Task<TripPaginationDto> GetAllAsync(string riderId, Expression<Func<Trip, object>> orderBy,
                                        bool descending = false, int pageNumber = 1, int pageSize = 10);
        Task RequestTripAsync(RequestTripDto dto);
        Task AcceptTripAsync(Guid tripId);
        Task EndTripAsync(Guid tripId);
        Task CancelTripAsync(Guid tripId);
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wasalnyy.DAL.Repo.Abstraction;
using Wasalnyy.DAL.Repo.Implementation;

namespace Wasalnyy.BLL.Service.Implementation
{
    public class AdminService : IAdminService
    {
        private readonly IDriverRepo _driverRepo;
        private readonly IRiderRepo _riderRepo;
        private readonly ITripRepo _tripRepo;
        private readonly IComplaintRepo _complaintRepo;
        private readonly IReviewRepo _reviewRepo;

        public AdminService(IDriverRepo driverRepo, IRiderRepo riderRepo, ITripRepo tripRepo, IComplaintRepo complaintRepo, IReviewRepo reviewRepo)
        {
            _driverRepo = driverRepo;
            _riderRepo = riderRepo;
            _tripRepo = tripRepo;
            _complaintRepo = complaintRepo;
            _reviewRepo = reviewRepo;
        }

        #region Driver Methods

        /// <summary>
        /// Get all drivers from the database
        /// </summary>
        public async Task<IEnumerable<Driver>> GetAllDriversAsync()
        {
            return await _driverRepo.GetAllDriverAsync();
        }

        /// <summary>
        /// Get a driver by their license number
        /// </summary>
        public async Task<Driver?> GetDriverByLicenseAsync(string licen)
        {
            return await _driverRepo.GetDriverByLicense(licen);
        }

        /// <summary>
        /// Get the number of trips completed by a driver
        /// </summary>
        public async Task<int> GetDriverTripCountAsync(string driverId)
        {
            return await _tripRepo.GetDriverTripsCountAsync(driverId);
        }

        /// <summary>
        /// Get all trips for a specific driver
        /// </summary>
        public async Task<IEnumerable<Trip>> GetDriverTripsAsync(string driverId)
        {
            return await _tripRepo.GetAllDriverTripsPaginatedAsync(
                driverId,
                orderBy: trip => trip.RequestedDate,
                descending: true,
                pageNumber: 1,
                pageSize: 50
            );
        }

        /// <summary>
        /// Get trips by driver license (not implemented)
        /// </summary>
        public Task<IEnumerable<Trip>> GetDriverTripsAsyncByLicen(string license)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get complaints submitted by a driver
        /// </summary>
        public async Task<IEnumerable<Complaint>> GetDriverSubmitedComplainsBylicenAsync(string licen)
        {
            return await _complaintRepo.DriverComplains(licen);
        }

        /// <summary>
        /// Get complaints filed against a driver
        /// </summary>
        public async Task<IEnumerable<Complaint>> GetDriverAgainstComplainsBylicenAsync(string licen)
        {
            return await _complaintRepo.DriverAgainstComplains(licen);
        }

        /// <summary>
        /// Get the average rating for a driver
        /// </summary>
        public async Task<double> GetDriverAvgRatingAsync(string licen)
        {
            var driver = await _driverRepo.GetDriverByLicense(licen);
            var rating = await _reviewRepo.GetDriverAverageRatingAsync(driver.Id);
            if(rating== 0)
            {
                rating = 0;
            }
            return rating;
        }

        /// <summary>
        /// Suspend a driver account by license
        /// </summary>
        public async Task SuspendAccountDriver(string lic)
        {
            var driver = await _driverRepo.GetDriverByLicense(lic);
            if (driver != null)
            {
                driver.Suspend();
                await _driverRepo.SaveChangesAsync();
            }
        }

        #endregion

        #region Rider Methods

        /// <summary>
        /// Get all riders from the database
        /// </summary>
        public async Task<IEnumerable<Rider>> GetAllRidersAsync()
        {
            return await _riderRepo.GetAllRidersAsync();
        }

        /// <summary>
        /// Get a rider by their phone number
        /// </summary>
        public async Task<Rider?> GetRiderByPhoneAsync(string phone)
        {
            return await _riderRepo.GetByPhoneAsync(phone);
        }

        /// <summary>
        /// Get the total count of riders
        /// </summary>
        public Task<double> GetRidersCount()
        {
            var count = _riderRepo.GetCountAsync();
            return count;
        }

        /// <summary>
        /// Get the number of trips completed by a rider
        /// </summary>
        public async Task<int> GetRiderTripCountAsync(string riderId)
        {
            return await _tripRepo.GetRiderTripsCountAsync(riderId);
        }

        /// <summary>
        /// Get all trips for a specific rider
        /// </summary>
        public async Task<IEnumerable<Trip>> GetRiderTripsAsync(string riderId)
        {
            return await _tripRepo.GetAllRiderTripsPaginatedAsync(
                riderId,
                orderBy: trip => trip.RequestedDate,
                descending: true,
                pageNumber: 1,
                pageSize: 50
            );
        }

        /// <summary>
        /// Get all trips for a rider by phone number
        /// </summary>
        public async Task<IEnumerable<Trip>> GetRiderTripsAsyncByphone(string phonenum)
        {
            return await _riderRepo.GetRiderTripsByPhone(phonenum);
        }

        /// <summary>
        /// Get all complaints filed by a rider
        /// </summary>
        public async Task<IEnumerable<Complaint>> GetRiderComplainsByPhoneAsync(string phonenum)
        {
            return await _riderRepo.GetRiderComplainsByPhone(phonenum);
        }

        /// <summary>
        /// Get a rider complaint by its ID
        /// </summary>
        public async Task<Complaint> GetRiderComplainByComplainsIdAsync(Guid id)
        {
            return await _complaintRepo.GetByIdAsync(id);
        }

        /// <summary>
        /// Suspend a rider account by ID
        /// </summary>
        public async Task SuspendAccountRider(string id)
        {
            var rider = await _riderRepo.GetByIdAsync(id);
            if (rider != null)
            {
                rider.Suspend();
                await _riderRepo.SaveChangesAsync();
            }
        }

        #endregion

        #region Trip Methods

        /// <summary>
        /// Get a trip by its ID
        /// </summary>
        public async Task<Trip?> GetTripByIdAsync(Guid id)
        {
            return await _tripRepo.GetByIdAsync(id);
        }

        /// <summary>
        /// Get all trips with a specific status
        /// </summary>
        public async Task<IEnumerable<Trip>> GetTripsByStatusAsync(TripStatus status)
        {
            return await _tripRepo.GetTripsByStatusAsync(status);
        }

        #endregion

        #region Reports/Statistics Methods

        /// <summary>
        /// Get the total count of drivers
        /// </summary>
        public async Task<int> GetTotalDriversAsync()
        {
            return await _driverRepo.GetCountAsync();
        }

        /// <summary>
        /// Get the total count of trips
        /// </summary>
        public async Task<int> GetTotalTripsAsync()
        {
            return await _tripRepo.GetCountAsync();
        }

        /// <summary>
        /// Get a complaint by its ID
        /// </summary>
        public async Task<Complaint> GetDriverComplainByComplainsIdAsync(Guid id)
        {
            return await _complaintRepo.GetByIdAsync(id);
        }

        #endregion
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Security.Claims;
using Wasalnyy.BLL.DTO.Rider;
using Wasalnyy.BLL.Service.Abstraction;
using Wasalnyy.BLL.Service.Implementation;

namespace Wasalnyy.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Rider")]
    public class RiderController : ControllerBase
    {
        private readonly IRiderService _riderService;
        private readonly IDriverService _driverService;
        private readonly ITripService _tripService;

        public RiderController(IRiderService riderService,IDriverService driverService, ITripService tripService)
        {
            _riderService = riderService;
            _driverService = driverService;
            _tripService = tripService;
        }



        [HttpGet("Profile")]
        public async Task<IActionResult> GetProfileAsync()
        {
            var riderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (riderId == null) return Unauthorized();

            var rider = await _riderService.GetByIdAsync(riderId);
            return Ok(rider);
        }

        [HttpGet("Name")]
        public async Task<IActionResult> GetNameAsync()
        {
            var riderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (riderId == null) return Unauthorized();

            var name = await _riderService.RiderName(riderId);
            return Ok(name);
        }

        [HttpGet("ProfileImage")]
        public async Task<IActionResult> GetProfileImageAsync()
        {
            var riderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (riderId == null) return Unauthorized();

            var image = await _riderService.RiderProfileImage(riderId);
            return Ok(image);
        }

        [HttpGet("TripsCount")]
        public async Task<IActionResult> GetTripsCountAsync()
        {
            var riderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (riderId == null) return Unauthorized();

            var count = await _riderService.RiderTotalTrips(riderId);
            return Ok(count);
        }

        [HttpGet("IsSuspended")]
        public async Task<IActionResult> IsSuspendedAsync()
        {
            var riderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (riderId == null) return Unauthorized();

            var suspended = await _riderService.IsRiderSuspended(riderId);
            return Ok(suspended);
        }

   

        [HttpPut("UpdateInfo")]
        public async Task<IActionResult> UpdateRider([FromBody] RiderUpdateDto riderUpdate)
        {
            var riderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (riderId == null) return Unauthorized();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _riderService.UpdateRiderInfo(riderId, riderUpdate);
            if (!updated)
                return NotFound(new { Message = "Rider not found." });

            return Ok(new { Message = "Rider information updated successfully." });
        }
        [HttpPost("DriverData")]
        public async Task<IActionResult> GetDriverDataAsync([FromBody] string driverId)
        {
            var driver = await _driverService.GetByIdAsync(driverId);
            if (driver == null)
               return Unauthorized();

            return Ok(driver);
        }

        [HttpGet("TripsHistory")]
        public async Task<IActionResult> GetTripsHistoryAsync(
            [FromQuery] string orderBy = "RequestedDate",
            [FromQuery] bool descending = false,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var riderd = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (riderd == null) return Unauthorized();

            Expression<Func<Trip, object>> orderByExpression = orderByExpression => orderByExpression.RequestedDate;
            orderBy = orderBy.ToLower();

            if (string.Equals(orderBy, "StartDate".ToLower()))
                orderByExpression = orderByExpression => orderByExpression.StartDate!;

            if (string.Equals(orderBy, "ArrivalDate".ToLower()))
                orderByExpression = orderByExpression => orderByExpression.ArrivalDate!;

            if (string.Equals(orderBy, "Price".ToLower()))
                orderByExpression = orderByExpression => orderByExpression.Price;

            return Ok(await _tripService.GetAllRiderTripsPaginatedAsync(riderd, orderByExpression, descending, pageNumber, pageSize));
        }
    }
}
